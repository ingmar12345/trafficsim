from controller.controller import Controller
from controller.controller import Algorithm
import random

class Randomizer( Algorithm ):
	"""
	Algorithm that randomly toggles a light every 15 seconds from red to green to orange to red again.
	"""

	def __init__( self ):
		super().__init__()
		self.counter = 0

	def calculateNewFloorplan( self ):
		# Count to 25 as to end up with about 5 seconds.
		if self.counter < 25:
			self.counter += 1
			return
		else:
			self.counter = 0

		modifiedLightCode = random.choice( list( self.controller.floorPlan.keys() ) )

		for light in Controller.ALL_KNOWN_TRAFFIC_LIGHTS:
			colour = self.controller.floorPlan[light]['colour']

			if colour == Controller.LIGHT_COLOR_GREEN or colour == Controller.LIGHT_COLOR_ORANGE:
				modifiedLightCode = light

		modifiedLight = self.controller.floorPlan[modifiedLightCode]

		if modifiedLight['colour'] == Controller.LIGHT_COLOR_RED:
			self.controller.floorPlan[modifiedLightCode]['colour'] = Controller.LIGHT_COLOR_GREEN
		elif modifiedLight['colour'] == Controller.LIGHT_COLOR_ORANGE:
			self.controller.floorPlan[modifiedLightCode]['colour'] = Controller.LIGHT_COLOR_RED
		else:
			self.controller.floorPlan[modifiedLightCode]['colour'] = Controller.LIGHT_COLOR_ORANGE

	def __str__( self ) -> str:
		return 'Randomizer'
