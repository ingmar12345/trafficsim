#!/usr/bin/env python
"""This is the controller entry point."""

import asyncio
import argparse
import os
import json
import websockets

from controller import logging
from controller.controller import Controller
from algorythms.manager import Manager

algorithmManager = Manager()

parser = argparse.ArgumentParser()
parser.add_argument(
	"--log-level",
	help = "level of logging to do",
	choices = logging.LogLevels.keys(),
	default = os.environ.get( 'CONTROLLER_LOG_LEVEL', None )
)
parser.add_argument(
	"--algorithm",
	help = "algorithm to use",
	choices = algorithmManager.getAlgorithmNames(),
	default = os.environ.get( 'CONTROLLER_ALGORITHM', 'Heuristic' )
)
parser.add_argument(
	"--port",
	help = "port to host the controller on. Defaults to 5678",
	type = int,
	default = os.environ.get( 'CONTROLLER_PORT', 5678 )
)
parser.add_argument(
	"--hostname",
	help = "host name. Defaults to 'localhost'",
	default = os.environ.get( 'CONTROLLER_HOST_NAME', 'localhost' )
)
parser.add_argument(
	"--log-to-console",
	help = 'whether logging should also be available in the console',
	action = 'store_true',
	default = False
)
parser.add_argument(
	"--send-strings",
	help = 'Send strings instead of byte arrays',
	action = 'store_true',
	default = bool( os.environ.get( 'CONTROLLER_SEND_STRINGS', False ) )
)

args = parser.parse_args()

logging.setupLoggers( args.log_level, args.log_to_console )
logger = logging.getLogger( 'script' )

logger.debug( 'Set hostname to %s with port %s', args.hostname, args.port )

# Process init
controller = Controller( logging.getLogger( 'controller' ) )
controller.prepareFloorPlan()
controller.setAlgorithm( algorithmManager.getAlgorithm( args.algorithm ) )

async def handleIncomingMessages( websocket, path ):
	async for message in websocket:
		logger.info( 'Received message: %s', message )

		if message is str:
			data = json.loads( message )
		else:
			data = json.loads( message, encoding = 'utf-8' )

		if type( data ) is dict:
			if 'quit' in data:
				await websocket.send( '{"quit": "acknowledged"}' )
				quit()
			else:
				# Don't choke on dictionaries
				data = list(data.values())
		else:
			# Remove duplicate entries by converting to a set
			data = set( data )

		for item in data:
			# Ignore echo'ed data.
			if type(item) is dict:
				return

			controller.registerTraffic( item.upper() )

async def processFloorplan( websocket, path ):
	while True:
		response = list( controller.computeFloorPlan() )

		# Set Allow_nan to False to ensure strict JSON standard compatibility
		if args.send_strings:
			response = json.dumps( response, allow_nan = False )
		else:
			response = json.dumps( response, allow_nan = False, ensure_ascii = False ).encode( 'utf-8' )

		logger.info( 'Send response: %s', response )

		await websocket.send( response )

		# Sleep for 200 milliseconds. 60 FPS would be 166 and 2/3.
		await asyncio.sleep( Controller.TICK_DURATION )

async def socketHandler( websocket, path ):
	message_processor_task = asyncio.ensure_future( handleIncomingMessages( websocket, path ) )
	floorplan_handler_task = asyncio.ensure_future( processFloorplan( websocket, path ) )

	done, pending = await asyncio.wait(
		[ message_processor_task, floorplan_handler_task ],
		return_when = asyncio.FIRST_COMPLETED
	)

	for task in pending:
		task.cancel()

start_server = websockets.serve( socketHandler, args.hostname, args.port )

asyncio.get_event_loop().run_until_complete( start_server )
asyncio.get_event_loop().run_forever()
