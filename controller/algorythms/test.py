from controller.controller import Controller
from controller.controller import Algorithm

class Test( Algorithm ):
	"""
	Algorithm that iteratively toggles each traffic light from red to green to orange to red again.
	"""

	def __init__( self ):
		super().__init__()
		self.counter = 0
		self.lightIndex = 0
		self.colour = Controller.LIGHT_COLOR_RED
		self.light = 'A1'

	def yieldLight( self ) -> str:
		"""
		Yield a traffic light code.
		Each call will return a different traffic light, looping back to A1 after reaching F2

		:return: Traffic light code
		:rtype: str
		"""
		self.lightIndex += 1

		lights = Controller.ALL_KNOWN_TRAFFIC_LIGHTS + Controller.TRAIN_PHANTOM_LIGHTS

		if self.lightIndex == len( lights ):
			self.lightIndex = 0


		return lights[self.lightIndex]

	def calculateNewFloorplan( self ):
		# Count to 25 as to end up with 5 seconds.
		if self.counter < 25:
			self.counter += 1
			return
		else:
			self.counter = 0

		updateLight = False

		if self.colour == Controller.LIGHT_COLOR_RED:
			self.colour = Controller.LIGHT_COLOR_GREEN
		elif self.colour == Controller.LIGHT_COLOR_ORANGE:
			self.colour = Controller.LIGHT_COLOR_RED
			updateLight = True
		elif self.colour == Controller.LIGHT_COLOR_GREEN:
			self.colour = Controller.LIGHT_COLOR_ORANGE

		print( 'Setting ' + self.light + ' to ' + self.colour )

		self.controller.floorPlan[self.light]['colour'] = self.colour

		if updateLight:
			self.light = self.yieldLight()

	def __str__( self ) -> str:
		return 'Test'
