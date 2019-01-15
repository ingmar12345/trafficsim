from typing import Dict
from typing import Iterator
from logging import Logger
from abc import ABC, abstractmethod
import math

class Algorithm( ABC ):
	"""
	Base class for algorithms.
	"""
	def __init__( self ):
		self.controller = None
		self.logger : Logger = None

	def setController( self, controller ):
		self.controller = controller

	def setLogger( self, logger : Logger ):
		self.logger = logger

	@abstractmethod
	def calculateNewFloorplan( self ):
		pass

	@abstractmethod
	def __str__(self) -> str:
		"""
		Return a human readable name.

		:rtype: str
		"""
		pass

class No( Algorithm ):
	"""
	Algorithm that doesn't calculate anything.
	"""
	def calculateNewFloorplan( self ):
		pass

	def __str__( self ) -> str:
		return 'No'

class Controller:
	"""
	Controller. Here happens magic!
	"""

	"""
	Duration between two cycles.
	:type: float
	"""
	TICK_DURATION : float = 0.2

	LIGHT_COLOR_RED : str = 'red'
	LIGHT_COLOR_ORANGE : str = 'orange'
	LIGHT_COLOR_YELLOW : str = LIGHT_COLOR_ORANGE
	LIGHT_COLOR_GREEN : str = 'green'
	LIGHT_COLOR_ONTRUIM_FLOORPLAN : str = 'If you see this, the controller screwed up somewhere'

	CAR_TRAFFIC_LIGHTS = [
		'A1',
		'A2',
		'A3',
		'A4',
		'A5',
		'A6',
		'A7',
		'A8',
		'A9',
		'A10'
	]

	CYCLIST_TRAFFIC_LIGHTS = [
		'B1',
		'B2',
		'B3'
	]

	PEDESTRIAN_TRAFFIC_LIGHTS = [
		'C1.1',
		'C1.2',
		'C2.1',
		'C2.2',
		'C3.1',
		'C3.2'
	]

	BUS_TRAFFIC_LIGHTS = [
		'D1'
	]

	TRAIN_TRAFFIC_LIGHTS = [
		'E1'
	]

	TRAIN_PHANTOM_LIGHTS = [
		'F1',
		'F2'
	]

	ALL_KNOWN_TRAFFIC_LIGHTS = CAR_TRAFFIC_LIGHTS + CYCLIST_TRAFFIC_LIGHTS + PEDESTRIAN_TRAFFIC_LIGHTS + BUS_TRAFFIC_LIGHTS + TRAIN_TRAFFIC_LIGHTS

	def __init__( self, logger : Logger ):
		self.floorPlan : Dict[str, Dict] = {}
		self.logger : Logger = logger
		self.algorithm : Algorithm = None
		self._setAlgorithm( No() )

	def _setAlgorithm( self, algorithm : Algorithm ):
		algorithm.setLogger( self.logger )
		algorithm.setController( self )
		self.algorithm = algorithm

	def setAlgorithm( self, algorithm : Algorithm ):
		self._setAlgorithm( algorithm )
		self.logger.info( 'Set to use algorithm ' + str( algorithm ) )

	def prepareFloorPlan( self ):
		"""
		Prepare the floorplan with all traffic lights, initializing them on red.
		"""
		for light in Controller.ALL_KNOWN_TRAFFIC_LIGHTS + Controller.TRAIN_PHANTOM_LIGHTS:
			self.floorPlan[light] = {
				'colour': Controller.LIGHT_COLOR_RED,
				'timer': None,
				'hasTraffic': False,
				'ticks': 0
			}

	def getFloorPlan( self ) -> Iterator[dict]:
		"""
		Get the current status of all lights.

		:rtype: Iterator[dict]
		"""
		for light, info in self.floorPlan.items():
			if info['colour'] == self.LIGHT_COLOR_ONTRUIM_FLOORPLAN:
				colour = self.LIGHT_COLOR_RED
			else:
				colour = info['colour']

			lightInfo = {
				'light': light,
				'status': colour
			}

			if info['timer'] is not None:
				lightInfo['timer'] = math.floor( info['timer'] * self.TICK_DURATION )

			yield lightInfo

	def computeFloorPlan( self ) -> Iterator[dict]:
		"""
		Calculate the new status of traffic lights and return the new status.

		:rtype: Iterator[dict]
		"""
		self.algorithm.calculateNewFloorplan()

		return self.getFloorPlan()

	def registerTraffic( self, target : str ):
		"""
		Registers traffic for the given traffic light.

		:param target: traffic light for which to register traffic
		:type target: string
		:return:
		"""
		target.replace( '_', '.' )
		# F1 & F2 are essentially E1. This controller does not discriminate between the direction.
		if target in Controller.TRAIN_PHANTOM_LIGHTS:
			self.registerTraffic( 'E1' )

		if not target in Controller.ALL_KNOWN_TRAFFIC_LIGHTS + Controller.TRAIN_PHANTOM_LIGHTS:
			self.logger.warning( 'Unsupported traffic light ' + target )
			return

		self.floorPlan[target]['hasTraffic'] = True
