import logging

"""
Logging levels supported by this program.
"""
LogLevels = {
	'CRITICAL': logging.CRITICAL,
	'FATAL': logging.FATAL,
	'ERROR': logging.ERROR,
	'WARN': logging.WARNING,
	'WARNING': logging.WARNING,
	'INFO': logging.INFO,
	'DEBUG': logging.DEBUG,
}
"""
Log level to use for any loggers created by this module.
"""
_globalLogLevel : str = None

def setupLoggers( logLevel : str, logToConsole : bool ):
	"""
	Define logging based on what logging level the user provides.
	When provided with None, logs will be dropped in the nearest volcano.

	:param logLevel: Logging level to log with. Log levels are found above, in LogLevels
	:type logLevel: str
	:param logToConsole: If the log contents should also be routed to the console
	:type logToConsole: bool
	"""
	global _globalLogLevel

	if logToConsole:
		logging.getLogger().addHandler( logging.StreamHandler() )

	# Always define the script logger
	formatter = logging.Formatter( fmt='%(asctime)s [%(levelname)s]: %(message)s', datefmt='%Y-%m-%d %H:%M:%S' )
	scriptLogger = logging.getLogger( 'script' )
	scriptLogger.setLevel( logging.DEBUG )
	fh = logging.FileHandler( 'logs/script.log' )
	fh.setFormatter( formatter )
	scriptLogger.addHandler( fh )
	scriptLogger.debug( 'Defined global log level as %s', logLevel )

	if logLevel is None:
		return

	_globalLogLevel = LogLevels[logLevel.upper()]

	# WebSockets specific
	wsLogger = logging.getLogger( 'websockets' )
	wsLogger.setLevel( _globalLogLevel )
	fh = logging.FileHandler( 'logs/websockets.log' )
	fh.setFormatter( formatter )
	wsLogger.addHandler( fh )

	# Controller specific
	cLogger = logging.getLogger( 'controller' )
	cLogger.setLevel( _globalLogLevel )
	fh = logging.FileHandler( 'logs/controller.log' )
	fh.setFormatter( formatter )
	cLogger.addHandler( fh )

def getLogger( logger : str ) -> logging.Logger:
	"""
	Obtain a logger.

	:param logger: Logging to obtain
	:type logger: str
	:returns logger for the requested channel
	:rtype Logger
	"""

	# Whenever no log level has been defined, logging for controller should just go to /dev/null
	if _globalLogLevel is None and logger is 'controller':
		return logging.getLogger( 'dummy' )
	else:
		return logging.getLogger( logger )
