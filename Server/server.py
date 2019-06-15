from typing import List
from http.server import BaseHTTPRequestHandler, HTTPServer
import json
from enum import Enum
import socket
import time
import datetime
import random

rooms = {}


# Debug class for easy logging with timestamps
class Debug():
	def __init__(self, console_output, file_output):
		self.console_output = console_output
		self.file_output = file_output

	def log(self, string: str, paragraph: bool = False, important: bool = False):
		ts = time.time()
		st = datetime.datetime.fromtimestamp(ts).strftime('%Y-%m-%d %H:%M:%S')
		if self.console_output:
			if paragraph:
				print("")
			new_string = string
			if important:
				new_string = "\033[92m"+string+"\033[00m"
            
			print("\033[91m["+st+"]\033[00m "+new_string)
			if paragraph:
				print("")
			

debug = Debug(console_output = True, file_output = False)

HOST = socket.gethostbyname(socket.gethostname())
PORT = 8000
debug.log('SERVER HOSTED ON: ' + HOST + ':' + str(PORT), False, False)


class GameState(Enum):
	lobby = 0
	started = 1
	playing = 2

class PlayerType(Enum):
	tobedetermined = 0
	criminal = 1
	cop = 2

class Point(object):
	def __init__(self, longitude, latitude):
		self.longitude = longitude
		self.latitude = latitude

	def __str__(self):
		return "long: "+str(self.longitude)+", lat: "+str(self.latitude)

class Playfield(object):
	def __init__(self, points: List[Point]):
		self.points = points # List of points that define the playfield
	
	def __str__(self):
		string = ""
		for point in self.points:
			string += "    ("+str(point)+")\n"
		return string
		

class Player(object):
	def __init__(self, ip: str, name: str, playertype: PlayerType):
		self.ip = ip
		self.name = name
		self.playertype = playertype

	def __str__(self):
		return "ip: "+self.ip+", name: "+self.name+", playertype: "+str(self.playertype)

class Room(object):
	def __init__(self, time: int, playfield: Playfield, host: Player):
		self.time = time # Time in seconds
		self.playfield = playfield
		self.playerlist = [host]

		self.pin = self.generate_pin()
	
	def generate_pin(self):
		length = 6
		new_pin = None
		while (new_pin == None or new_pin in rooms):
			
			new_pin = ""
			for _ in range(length):
				number = random.randint(0, 9)
				new_pin += str(number)
		return new_pin


	def __str__(self):
		string = "\n[Room #"+self.pin+"]\n  Time:\n    "+str(self.time)+"\n  Players: \n"
		
		for player in self.playerlist:
			string += "    ("+str(player)+")\n"

		string += "  Playfield: \n"+str(self.playfield)
		return string


def handle_json(json_data):
	global rooms
	#print(json_data)
	#try:
	if json_data['action'] == 'create_game':

		debug.log("Create game", important = True)

		time = json_data['time']

		pointsRaw = json_data['playfield']
		points = []
		for point in pointsRaw:
			points.append(Point(point['longitude'], point['latitude']))

		playfield = Playfield(points)

		host = Player(json_data['ip'], json_data['name'], PlayerType.tobedetermined)

		new_room = Room(time, playfield, host)

		rooms[new_room.pin] = new_room

		debug.log("New room created: "+str(new_room))

		return {'status': 'success', 'room_pin': new_room.pin}

	elif json_data['action'] == 'join_game':

		debug.log("Join game", important=True)

		player = Player(json_data['ip'], json_data['name'], PlayerType.tobedetermined)

		room_pin = json_data['room_pin']

		if room_pin in rooms:
			rooms[room_pin].playerlist.append(player)

			debug.log(str(rooms[room_pin]))
			return {'status': 'success', 'room_pin': room_pin}
		else:
			return {'status': 'failed'}
	elif json_data['action'] == 'leave_game':

		debug.log("Leave game", important=True)

		room_pin = json_data['room_pin']

		length = len(rooms[room_pin].playerlist)

		if length > 1:
			# This means that the player can be removed

			playerlist = rooms[room_pin].playerlist
			for i, player in enumerate(playerlist):
				debug.log(player.ip)
				debug.log(json_data['ip'])
				if player.ip == json_data['ip'] and player.name == json_data['name']:
					
					del playerlist[i]

					debug.log(str(rooms[room_pin]))
					return {'status': 'success'}

			
		else:
			# This means the room needs to be removed
			del rooms[room_pin]
			debug.log("Room #"+str(room_pin)+" deleted")
			return {'status': 'success'}
			
		return {'status': 'failed'}
		
	#except KeyError:
#		pass
#	return {'status': 'failed', 'message': 'Something went wrong. '}

class RequestHandler(BaseHTTPRequestHandler):

	def do_POST(self):
		data_string = self.rfile.read(int(self.headers['Content-Length']))
		json_data = json.loads(data_string.decode())
		json_data.update({'ip': self.client_address[0]})
		debug.log("Incoming json_data: "+str(json_data), True)
		self.send_response(200)
		self.send_header('Content-type', 'application/json')
		self.end_headers()
		outgoing_json_data = json.dumps(handle_json(json_data)).encode()
		debug.log("Outgoing json_data: "+str(outgoing_json_data), True)
		self.wfile.write(outgoing_json_data)

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
