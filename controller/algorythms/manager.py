from typing import Dict
from typing import List
from controller.controller import Algorithm
from algorythms.randomizer import Randomizer
from algorythms.heuristic import Heuristic
from algorythms.disco import Disco
from algorythms.zondagsrust import ZondagsRust
from algorythms.specific import Specific
from algorythms.test import Test
from algorythms.all import All

class Manager:
	def __init__( self ):
		algorithms : List[Algorithm] = [
			All(),
			Randomizer(),
			Heuristic(),
			Disco(),
			ZondagsRust(),
			Specific(),
			Test()
		]

		self.algorithms : Dict[str, Algorithm] = {}

		for algorithm in algorithms:
			name: str = str( algorithm )
			self.algorithms[name] = algorithm

	def getAlgorithmNames( self ) -> List[str]:
		"""
		Return a list with all supported algorithms.

		:rtype: List[str]
		"""
		return list( self.algorithms.keys() )

	def getAlgorithm( self, name : str ) -> Algorithm:
		"""
		Retrieve an algorithm

		:param name: Name of the algorithm
		:type: name: str
		:raises NameError when the algorithm is not known
		:return: The algorithm with the given name
		:rtype: Algorithm
		"""
		if not name in self.getAlgorithmNames():
			raise NameError( "Not a known algorithm: " + name )

		return self.algorithms[name]
