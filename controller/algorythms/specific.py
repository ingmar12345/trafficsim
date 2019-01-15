from controller.controller import Controller
from controller.controller import Algorithm

class Specific( Algorithm ):
	"""
	Algorithm that toggles (a) specific light(s) from red to green to orange to red again.
	"""

	def __init__( self ):
		super().__init__()
		self.counter = 0
		self.colour = Controller.LIGHT_COLOR_GREEN
		self.groups = [
			'D1'
		]

	def calculateNewFloorplan( self ):
		# Count to 30 as to end up with about 6 seconds.
		if self.counter < 30:
			self.counter += 1
			return
		else:
			self.counter = 0

		if self.colour == Controller.LIGHT_COLOR_RED:
			self.colour = Controller.LIGHT_COLOR_GREEN
		elif self.colour == Controller.LIGHT_COLOR_ORANGE:
			self.colour = Controller.LIGHT_COLOR_RED
		elif self.colour == Controller.LIGHT_COLOR_GREEN:
			self.colour = Controller.LIGHT_COLOR_ORANGE

		for light in self.groups:
			self.controller.floorPlan[light]['colour'] = self.colour

	def __str__( self ) -> str:
		return 'Specific'
