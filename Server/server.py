from http.server import BaseHTTPRequestHandler, HTTPServer
import json
from enum import Enum

HOST = '172.20.10.12'
PORT = 80

class GameState(Enum):
	lobby = 0
	started = 1
	playing = 2

class PlayerType(Enum):
	criminal = 0
	cop = 1

rooms = []

class Playfield(object):
	def __init__(self, points):
		self.points = points # List of points that define the playfield

class Player(object):
	def __init__(self, ip, name, playertype):
		self.ip = ip
		self.name = name
		self.playertype = playertype

class Room(object):
	def __init__(self, time, playfield, host):
		self.time = time # Time in seconds
		self.playfield = playfield
		self.playerlist

	def addPlayer(self):
		pass

	def kickPlayer(self):
		pass






def create_room():

	time = 10*60

	playfield = Playfield()

	room = Room()
	rooms.append(room)


'''
Room structure

[
	[
		id (string), 
		state (bool), 
		time (int)
		time_left (int), 
		map_points (array) [
			(longitude (float), latitude (float),
			...
		],
		start_point (longitude (float), latitude (float)),
		players (array) [
			player (array) [
				[criminal (bool), longitude (float), latitude (float)
			]
		]
	]
]

'''

#def handle_json(json_data):
#	if json_data['action'] == 'create_room':