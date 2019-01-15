from controller.controller import Controller
from controller.controller import Algorithm
import random

class Disco( Algorithm ):
	"""
	Algorithm that randomly toggles lights.
	"""

	def calculateNewFloorplan( self ):
		colours = [
			Controller.LIGHT_COLOR_RED,
			Controller.LIGHT_COLOR_ORANGE,
			Controller.LIGHT_COLOR_GREEN
		]

		for light in Controller.ALL_KNOWN_TRAFFIC_LIGHTS:
			self.controller.floorPlan[light] = {
				'colour': random.choice( colours ),
				'timer': None,
				'hasTraffic': False
			}

	def __str__( self ) -> str:
		return 'Disco'
