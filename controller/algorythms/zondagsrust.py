from controller.controller import Controller
from controller.controller import Algorithm

class ZondagsRust( Algorithm ):
	"""
	Algorithm that toggles all traffic lights to orange.
	"""

	def calculateNewFloorplan( self ):
		for light in Controller.CAR_TRAFFIC_LIGHTS + Controller.CYCLIST_TRAFFIC_LIGHTS + Controller.BUS_TRAFFIC_LIGHTS:
			self.controller.floorPlan[light] = {
				'colour': Controller.LIGHT_COLOR_ORANGE,
				'timer': -1,
				'hasTraffic': False
			}

		for light in Controller.PEDESTRIAN_TRAFFIC_LIGHTS:
			self.controller.floorPlan[light] = {
				'colour': Controller.LIGHT_COLOR_GREEN,
				'timer': -1,
				'hasTraffic': False
			}

	def __str__( self ) -> str:
		return 'ZondagsRust'
