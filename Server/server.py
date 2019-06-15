from typing import List
from http.server import BaseHTTPRequestHandler, HTTPServer
import json
from enum import Enum
import socket
import time
import datetime


# Debug class for easy logging with timestamps
class Debug():
	def __init__(self, console_output, file_output):
		self.console_output = console_output
		self.file_output = file_output

	def log(self, string: str):
		ts = time.time()
		st = datetime.datetime.fromtimestamp(ts).strftime('%Y-%m-%d %H:%M:%S')
		if self.console_output:
			print("["+st+"] "+string)
debug = Debug(console_output = True, file_output = False)

HOST = socket.gethostbyname(socket.gethostname())
PORT = 8000
debug.log('SERVER HOSTED ON: ' + HOST + ':' + str(PORT))


class GameState(Enum):
	lobby = 0
	started = 1
	playing = 2

class PlayerType(Enum):
	tobedetermined = 0
	criminal = 1
	cop = 2

rooms = []

class Point(object):
	def __init__(self, longitude, latitude):
		self.longitude = longitude
		self.latitude = latitude

class Playfield(object):
	def __init__(self, points: List[Point]):
		self.points = points # List of points that define the playfield

class Player(object):
	def __init__(self, ip: str, name: str, playertype: PlayerType):
		self.ip = ip
		self.name = name
		self.playertype = playertype

class Room(object):
	def __init__(self, time: int, playfield: Playfield, host: Player):
		self.time = time # Time in seconds
		self.playfield = playfield
		self.playerlist = [host]

		self.pin = '000000'

	def __str__(self):
		return "object of \"Room\" class"


	def addPlayer(self):
		pass

	def kickPlayer(self):
		pass






def create_room():

	time = 10*60

	playfield = Playfield([])

	room = Room(time, playfield, Player('0', 'Arjo', PlayerType.tobedetermined))
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
{
	'action': 'create_room',
	'time': 120,
	'playfield': 'test',
	'ip': '0.0.0.0',
	'name': 'Arjo'
}

{'action': 'create_room', 'time': 120, 'playfield': 'test', 'ip': '0.0.0.0', 'name': 'Arjo'}
'''


def handle_json(json_data):
	#print(json_data)
	#try:
	if json_data['action'] == 'test_connection':
		return {'answer': 'Dit is een test'}
	if json_data['action'] == 'create_game':

		debug.log("Create game")

		time = json_data['time']

		pointsRaw = json_data['playfield']
		points = []
		for point in pointsRaw:
			points.append(Point(point['longitude'], point['latitude']))

		playfield = Playfield(points)

		host = Player(json_data['ip'], json_data['name'], PlayerType.tobedetermined)

		new_room = Room(time, playfield, host)

		debug.log("New room created: "+str(new_room))

		return {'status': 'success', 'room_pin': new_room.pin}

	elif json_data['action'] == 'join_room':
		# json_data['room_pin']
		pass
		
	#except KeyError:
#		pass
#	return {'status': 'failed', 'message': 'Something went wrong. '}

class RequestHandler(BaseHTTPRequestHandler):

	def do_GET(self):
		debug.log("HTTP GET")

	def do_POST(self):
		debug.log("HTTP POST")
		data_string = self.rfile.read(int(self.headers['Content-Length']))
		#print(data_string)
		json_data = json.loads(data_string.decode())
		self.send_response(200)
		self.send_header('Content-type', 'application/json')
		self.end_headers()
		self.wfile.write(json.dumps(handle_json(json_data)).encode())

	def log_message(self, format, *args):
		pass


def main():
	try:
		server = HTTPServer((HOST, PORT), RequestHandler)
		server.serve_forever()

	except KeyboardInterrupt:
		pass

if __name__ == '__main__':
	main()
