from controller.controller import Controller
from controller.controller import Algorithm

class All( Algorithm ):
	"""
	Algorithm that toggles all lights at the same time.
	"""

	def __init__( self ):
		super().__init__()
		self.counter = 0
		self.colour = Controller.LIGHT_COLOR_RED

	def calculateNewFloorplan( self ):
		# Count to 35 as to end up with about 5 seconds.
		if self.counter < 35:
			self.counter += 1
			return
		else:
			self.counter = 0

		updateLight = False

		if self.colour == Controller.LIGHT_COLOR_RED:
			self.colour = Controller.LIGHT_COLOR_GREEN
		elif self.colour == Controller.LIGHT_COLOR_ORANGE:
			self.colour = Controller.LIGHT_COLOR_RED
		elif self.colour == Controller.LIGHT_COLOR_GREEN:
			self.colour = Controller.LIGHT_COLOR_ORANGE

		for light in self.controller.ALL_KNOWN_TRAFFIC_LIGHTS:
			self.controller.floorPlan[light]['colour'] = self.colour

	def __str__( self ) -> str:
		return 'All'
