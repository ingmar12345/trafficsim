import random
from typing import Dict
from typing import List
from controller.controller import Controller
from controller.controller import Algorithm

class Heuristic( Algorithm ):
	"""
	List of lanes that cross the given lane.
	These are used to determine if a given traffic light can go green and
	which lights should go red when traffic has been waiting for too long.
	"""
	FreeMap : Dict[str, List[str]] = {
		  'A1': ['A5', 'A7', 'B1', 'C1.1', 'C3.2', 'D1'],
		  'A2': ['A5', 'A7', 'A8', 'A9', 'A10', 'B1', 'C1.1', 'D1', 'E1'],
		  'A3': ['A5', 'A6', 'A7', 'A9', 'A10', 'B1', 'B2', 'C1.1', 'C2.2', 'D1'],
		  'A4': ['A6', 'A10', 'B1', 'B2', 'C2.1', 'C1.2', 'D1'],
		  'A5': ['A1', 'A2', 'A3', 'A6', 'A7', 'A10', 'B1', 'B3', 'C2.1', 'C3.2', 'D1'],
		  'A6': ['A3', 'A4', 'A5', 'A9', 'A10', 'B1', 'B2', 'C1.2', 'C2.2', 'D1', 'E1'],
		  'A7': ['A1', 'A2', 'A3', 'A5', 'A9', 'A10', 'B3', 'C3.2', 'D1', 'E1'],
		  'A8': ['A2', 'A7', 'B3', 'C3.1', 'E1'],
		  'A9': ['A2', 'A3', 'A6', 'A7', 'B2', 'B3', 'C2.2', 'C3.1'],
		 'A10': ['A2', 'A3', 'A4', 'A5', 'A6', 'A7', 'B2', 'B3', 'C1.2', 'C3.2', 'D1'],
		  'B1': ['A1', 'A2', 'A3', 'A4', 'A6', 'A10'],
		  'B2': ['A3', 'A4', 'A5', 'A6', 'A9', 'D1'],
		  'B3': ['A1', 'A5', 'A7', 'A8', 'A9', 'A10', 'D1'],
		'C1.1': ['A1', 'A2', 'A3'],
		'C1.2': ['A4', 'A6', 'A10'],
		'C2.1': ['A4', 'A5', 'D1'],
		'C2.2': ['A3', 'A6', 'A9'],
		'C3.1': ['A8', 'A9', 'A10'],
		'C3.2': ['A1', 'A5', 'A7', 'D1'],
		  'D1': ['A1', 'A2', 'A3', 'A4', 'A5', 'A6', 'A7', 'A10', 'B2', 'B3', 'C2.1', 'C3.2'],
		  # Included for ease of use, but beware that the train does not stop for these lanes!
		  'E1': ['A2', 'A6', 'A7', 'A8'],
		  'F1': ['A2', 'A6', 'A7', 'A8'],
		  'F2': ['A2', 'A6', 'A7', 'A8']
	}

	RED : str = Controller.LIGHT_COLOR_RED
	ORANGE : str = Controller.LIGHT_COLOR_ORANGE
	GREEN : str = Controller.LIGHT_COLOR_GREEN
	ONTRUIM_FLOORPLAN : str = Controller.LIGHT_COLOR_ONTRUIM_FLOORPLAN

	def __init__( self ):
		super().__init__()

		# E1, F1 & F2 are last in the list, but shouldn't be managed here.
		# They only should do something when the simulator flags a train as approaching.
		self.lightEvaluationOrder : List[str] = list( self.FreeMap.keys() )[:-3]
		self.lightWithPriority : str = None
		self.lastLightWithPriorityResolverCounter: int = 0

	def calculateNewFloorplan( self ):
		"""
		Calculate the new traffic light colours.
		"""
		self.lastLightWithPriorityResolverCounter -= 1
		# Do some simple stuff, like:
		## Turn an orange light red when it has been orange for the duration the specific light should stay orange.
		## Turn a green light orange when it has been green for the duration the specific light should stay green.
		for light, info in self.controller.floorPlan.items():
			info['ticks'] += 1
			if info['timer'] is not None:
				info['timer'] -= 1

			orangeDuration = self.getOrangeLightDuration( light )
			greenDuration = self.getGreenLightDuration( light )

			if info['colour'] == self.ONTRUIM_FLOORPLAN and info['ticks'] > 10:
				self.setLight( light, self.RED )
				if light == 'E1':
					if self.controller.floorPlan['F1']['colour'] == self.ONTRUIM_FLOORPLAN:
						self.setLight( 'F1', self.RED )
					elif self.controller.floorPlan['F2']['colour'] == self.ONTRUIM_FLOORPLAN:
						self.setLight( 'F2', self.RED )
			if info['colour'] == self.ORANGE and info['ticks'] > orangeDuration:
				self.setLight( light, self.ONTRUIM_FLOORPLAN )
				if light == 'E1':
					if self.controller.floorPlan['F1']['colour'] == self.ORANGE:
						self.setLight( 'F1', self.ONTRUIM_FLOORPLAN )
					elif self.controller.floorPlan['F2']['colour'] == self.ORANGE:
						self.setLight( 'F2', self.ONTRUIM_FLOORPLAN )
			elif info['colour'] == self.GREEN and info['ticks'] > greenDuration:
				self.setLight( light, self.ORANGE, orangeDuration )
				# Set hasTraffic to false now, so that any report of new traffic will be caught during the orange state.
				info['hasTraffic'] = False
				# Don't set the opposing F light to orange too when it wasn't green to begin with.
				if light == 'E1':
					if self.controller.floorPlan['F1']['hasTraffic']:
						self.controller.floorPlan['F1']['hasTraffic'] = False
						self.setLight( 'F1', self.ORANGE, orangeDuration )
					elif self.controller.floorPlan['F2']['hasTraffic']:
						self.controller.floorPlan['F2']['hasTraffic'] = False
						self.setLight( 'F2', self.ORANGE, orangeDuration )

		if self.controller.floorPlan['D1']['hasTraffic'] and self.controller.floorPlan['D1']['colour'] == self.RED:
			if not self.pathIsFreeFor( 'D1' ):
				# It has been 20 seconds, people've got a train to catch!
				# Force the other traffic off the intersection and make way for the bus.
				if self.controller.floorPlan['D1']['ticks'] > 100:
					self.logger.info( 'D1 waiting too long: forcing crossing lanes to red' )
					self.setCrossingLanesToRed( 'D1' )
			else:
				greenDuration = self.getGreenLightDuration( 'D1' )
				self.setLight( 'D1', self.GREEN, greenDuration )

		# Trains don't stop. Ensure all lights that route to the intersection will be red when the train passes.
		if self.controller.floorPlan['E1']['hasTraffic'] and self.controller.floorPlan['E1']['colour'] == self.RED:
			if not self.pathIsFreeFor( 'E1' ):
				self.logger.info( 'Path is not free for E1, forcing crossing lanes to red' )
				self.setCrossingLanesToRed( 'E1' )
			else:
				greenDuration = self.getGreenLightDuration( 'E1' )
				self.setLight( 'E1', self.GREEN, greenDuration )
				# Apparently, F2 has priority.
				if self.controller.floorPlan['F2']['hasTraffic']:
					self.setLight( 'F2', self.GREEN, greenDuration )
				elif self.controller.floorPlan['F1']['hasTraffic']:
					self.setLight( 'F1', self.GREEN, greenDuration )

		highestPriority = 0

		for light in self.lightEvaluationOrder:
			info = self.controller.floorPlan[light]
			if info['colour'] == self.RED and info['hasTraffic'] and info['ticks'] > 600 and info['ticks'] > highestPriority:
				self.logger.info( 'Noting %s as having waited too long. Forcing crossing lanes to red', light )
				highestPriority = info['ticks']
				self.lightWithPriority = light

		# Shuffle the list of lights in order to randomize which light gets picked first,
		# to prevent A1 getting priority when it shouldn't.
		random.shuffle( self.lightEvaluationOrder )

		if self.lightWithPriority is not None:
			self.lastLightWithPriorityResolverCounter = self.getGreenLightDuration( self.lightWithPriority )
			lightsToEvaluate = [ self.lightWithPriority ] + self.lightEvaluationOrder
		else:
			lightsToEvaluate = self.lightEvaluationOrder

		for light in lightsToEvaluate:
			info = self.controller.floorPlan[light]

			if info['colour'] == self.RED:
				greenDuration = self.getGreenLightDuration( light )
				if info['hasTraffic']:
					if self.pathIsFreeFor( light ):
						self.logger.info( 'Path is free for %s setting to green', light )
						self.setLight( light, self.GREEN, greenDuration )
					# Oh noes! People have been waiting for 2 minutes and they've still not gotten green!
					# They might be in a hurry, better give them some green.
					elif info['ticks'] > 600 and self.lastLightWithPriorityResolverCounter <= 0:
						self.setCrossingLanesToRed( light )
				# We've seen enough red, lets see some green instead!
				# This is mainly here to assist simulators that have trouble letting know if traffic is waiting.
				# After about 60 seconds, a light will go green when it it safe to do so.
				# Also, it confuses the drivers, so that's always a plus.
				elif self.pathIsFreeFor( light ) and info['ticks'] > self.randomJumpGreenTime( light ) and light != 'D1':
					self.logger.info( 'Randomly setting %s to green', light )
					self.setLight( light, self.GREEN, greenDuration )

	def setCrossingLanesToRed( self, light : str ):
		"""
		Set all lanes crossing the given light to red.
		This toggles them to orange ONLY when they are green,
		to prevent overwriting them all the time and causing them to be stuck on orange.

		:param light: traffic light code
		:type light: str
		"""
		for laneLight in self.FreeMap[light]:
			if self.controller.floorPlan[laneLight]['colour'] == self.GREEN:
				orangeDuration = self.getOrangeLightDuration( laneLight )
				self.setLight( laneLight, self.ORANGE, orangeDuration )

	def setLight( self, light : str, colour : str, timerDuration = None ):
		"""
		Set a specific light to a specific colour.
		This also reset the timer for that light back to 0.

		:param light: traffic light code
		:type light: str
		:param colour: traffic light colour
		:type colour: str
		:param timerDuration:
		:type timerDuration: int
		"""
		self.controller.floorPlan[light]['colour'] = colour
		self.controller.floorPlan[light]['ticks'] = 0
		self.controller.floorPlan[light]['timer'] = timerDuration

	def pathIsFreeFor( self, light : str ) -> bool:
		"""
		Determines if the paths crossing light are free (set to red).

		:param light: traffic light code
		:type light: str
		:return: If the paths crossing light are free
		:rtype bool
		"""
		for blocker in self.FreeMap[light]:
			if self.controller.floorPlan[blocker]['colour'] != self.RED:
				return False

		return True

	def getOrangeLightDuration( self, light : str ) -> int:
		"""
		Determines the duration (in ticks) the given light should remain orange.

		:param light: traffic light code for which to determine the duration
		:type light: str
		:return: Amount of ticks the given light should be orange
		:rtype int
		"""
		group = light[0]
		if group == 'F':
			group = 'E'

		durations = {
			'A': 20, # 4 seconds
			'B': 10, # 2 seconds
			'C': 10, # 2 seconds
			'D': 20, # 4 seconds
			'E': 10  # 2 seconds
		}

		return durations[group]

	def getGreenLightDuration( self, light : str ) -> int:
		"""
		Determines the duration (in ticks) the given light should remain green.

		:param light: traffic light code for which to determine the duration
		:type light: str
		:return: Amount of ticks the given light should be green
		:rtype int
		"""
		group = light[0]
		if group == 'F':
			group = 'E'

		durations = {
			'A': 35, # 7 seconds
			'B': 35, # 7 seconds
			'C': 30, # 5 seconds
			'D': 25, # 5 seconds
			'E': 120 # 25 seconds
		}

		return durations[group]

	def randomJumpGreenTime( self, light : str ) -> int:
		"""
		Determines the duration (in ticks) after which the given light should go green.
		This is mainly here to assist simulators that have trouble letting know if traffic is waiting.

		:param light: traffic light code for which to determine the duration
		:type light: str
		:return: Amount of ticks
		:rtype int
		"""
		group = light[0]
		if group == 'F':
			group = 'E'

		durations = {
			'A': 300, # 60 seconds
			'B': 500, # 100 seconds
			'C': 600, # 120 seconds
			'D': -1,  # Not supported
			'E': -1   # Not supported
		}

		return durations[group]

	def __str__( self ) -> str:
		return 'Heuristic'
