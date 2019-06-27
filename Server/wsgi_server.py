from typing import List
import json
from enum import Enum
import time
import datetime
import random
import geopy.distance

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


debug = Debug(console_output=True, file_output=False)


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
		self.points = points  # List of points that define the playfield
		self.criminal_start_position = Point(0, 0)
		self.cop_start_position = Point(0, 0)
		self.max_distance = 120

	def __str__(self):
		string = ""
		for point in self.points:
			string += "    ("+str(point)+")\n"
		return string


class Player(object):
	def __init__(self, ip: str, name: str, is_host: bool, playertype: PlayerType):
		self.ip = ip
		self.name = name
		self.is_host = is_host
		self.playertype = playertype
		self.is_ready = False
		self.position = Point(0, 0)

	def __str__(self):
		return "ip: "+self.ip+", name: "+self.name+", playertype: "+str(self.playertype)


class Room(object):
	def __init__(self, game_time, playfield: Playfield, host: Player):
		self.game_time = game_time  # Time in seconds
		self.playfield = playfield
		self.playerlist = [host]
		self.starting = False
		self.busy = False
		self.start_timestamp = 0
		self.start_delay = 0
		self.game_started = False
		self.ciminal_amount = 1
		self.caught = False
		self.game_start_timestamp = 0
		self.stopped = False

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
		string = "\n[Room #"+self.pin+"]\n  Time:\n    " + \
			str(self.game_time)+"\n  Players: \n"

		for player in self.playerlist:
			string += "    ("+str(player)+")\n"

		string += "  Playfield: \n"+str(self.playfield)
		return string


def handle_json(json_data):
	global rooms

	if json_data['action'] == 'test_connection':
		return {'status': 'success'}

	elif json_data['action'] == 'create_game':

		debug.log("Create game", important=True)

		game_time = json_data['time']

		pointsRaw = json_data['playfield']
		points = []
		for point in pointsRaw:
			points.append(Point(point['longitude'], point['latitude']))

		playfield = Playfield(points)

		host = Player(json_data['ip'], json_data['name'],
		              True, PlayerType.tobedetermined)

		new_room = Room(game_time, playfield, host)

		rooms[new_room.pin] = new_room

		playerlist = new_room.playerlist
		playerlistRaw = []
		for player in playerlist:
			playerlistRaw.append(
				{'ip': player.ip, 'name': player.name, 'is_host': player.is_host})

		debug.log("New room created: "+str(new_room))

		return {'status': 'success', 'ip': json_data['ip'], 'name': json_data['name'], 'room_pin': new_room.pin, 'time': game_time, 'playfield': pointsRaw, 'playerlist': playerlistRaw}

	elif json_data['action'] == 'join_game':

		debug.log("Join game", important=True)

		player = Player(json_data['ip'], json_data['name'],
		                False, PlayerType.tobedetermined)

		room_pin = json_data['room_pin']

		if room_pin in rooms:
			room = rooms[room_pin]

			if room.busy:
				return {'status': 'busy'}
			room.playerlist.append(player)

			debug.log(str(rooms[room_pin]))

			game_time = rooms[room_pin].game_time
			points = rooms[room_pin].playfield.points
			pointsRaw = []
			for point in points:
				pointsRaw.append({'longitude': point.longitude,
                                    'latitude': point.latitude})

			playerlist = rooms[room_pin].playerlist
			playerlistRaw = []
			for player in playerlist:
				playerlistRaw.append(
					{'ip': player.ip, 'name': player.name, 'is_host': player.is_host})

			return {'status': 'success', 'ip': json_data['ip'], 'name': json_data['name'], 'room_pin': room_pin, 'time': game_time, 'playfield': pointsRaw, 'playerlist': playerlistRaw}
		else:
			return {'status': 'failed'}

	elif json_data['action'] == 'leave_game':

		debug.log("Leave game", important=True)

		room_pin = json_data['room_pin']
		room = rooms[room_pin]

		length = len(room.playerlist)

		if length > 1:
			# This means that the player can be removed

			if room.starting:
				return {'status': 'room_starting'}

			playerlist = room.playerlist
			for i, player in enumerate(playerlist):
				if player.ip == json_data['ip'] and player.name == json_data['name']:

					del playerlist[i]
					playerlist[0].is_host = True

					debug.log(str(room))
					return {'status': 'success'}

		else:
			# This means the room needs to be removed
			del room
			debug.log("Room #"+str(room_pin)+" deleted")
			return {'status': 'success'}

		return {'status': 'failed'}

	elif json_data['action'] == 'kick_player':

		debug.log("Kick player", important=True)

		room_pin = json_data['room_pin']

		playerlist = rooms[room_pin].playerlist
		for i, player in enumerate(playerlist):
			if player.ip == json_data['kick_ip'] and player.name == json_data['kick_name']:

				del playerlist[i]

				debug.log(str(rooms[room_pin]))
				return {'status': 'success', 'kick_name': json_data['kick_name']}
		return {'status': 'failed'}

	elif json_data['action'] == 'update_room_data':

		debug.log("Update room data", important=True)

		room_pin = json_data['room_pin']

		if room_pin in rooms:
			room = rooms[room_pin]

			points = room.playfield.points
			pointsRaw = []
			for point in points:
				pointsRaw.append({'longitude': point.longitude,
                                    'latitude': point.latitude})

			starting = room.starting

			delay = 0

			if (starting):
				delay = room.start_timestamp - time.time() + room.start_delay

			playerlist = room.playerlist
			playerlistRaw = []
			for player in playerlist:
				playertype_int = 0
				if player.playertype == PlayerType.cop:
					playertype_int = 1
				elif player.playertype == PlayerType.criminal:
					playertype_int = 2

				playerlistRaw.append(
					{'ip': player.ip, 'name': player.name, 'is_host': player.is_host, 'playertype': playertype_int})

			playertype_int = 0
			for player in playerlist:
				if player.ip == json_data['ip'] and player.name == json_data['name']:
					if player.playertype == PlayerType.cop:
						playertype_int = 1
					elif player.playertype == PlayerType.criminal:
						playertype_int = 2

			return {'status': 'success', 'playerlist': playerlistRaw, 'starting': starting, 'delay': delay, 'playertype': playertype_int}
		else:
			return {'status': 'failed'}

	elif json_data['action'] == 'request_start_game':

		debug.log("Request start game", important=True)

		room_pin = json_data['room_pin']

		if room_pin in rooms:
			room = rooms[room_pin]
            
			if len(room.playerlist) < 2:
				return {'status': 'not_enough_players'}
			
			if room.starting == True:
				return {'status': 'already_starting'}

			room.starting = True
			room.busy = True
			room.start_delay = json_data['delay']
			room.start_timestamp = time.time()

			# Random player assignment

			for player in room.playerlist:
				player.playertype = PlayerType.cop

			for i in range(room.ciminal_amount):

				random_int = random.randint(0, len(room.playerlist)-1)
				while (room.playerlist[random_int].playertype == PlayerType.criminal):
					random_int = random.randint(0, len(room.playerlist)-1)
				room.playerlist[random_int].playertype = PlayerType.criminal

			# Create playerlist json
			playerlist = room.playerlist
			playerlistRaw = []
			for player in playerlist:
				playertype_int = 0
				if player.playertype == PlayerType.cop:
					playertype_int = 1
				elif player.playertype == PlayerType.criminal:
					playertype_int = 2

				playerlistRaw.append(
					{'ip': player.ip, 'name': player.name, 'is_host': player.is_host, 'playertype': playertype_int})

			# Send playertype of player
			playertype_int = 0
			for player in playerlist:
				if player.ip == json_data['ip'] and player.name == json_data['name']:
					if player.playertype == PlayerType.cop:
						playertype_int = 1
					elif player.playertype == PlayerType.criminal:
						playertype_int = 2

			return {'status': 'success', 'playerlist': playerlistRaw, 'playertype': playertype_int}

		else:
			return {'status': 'failed'}
	elif json_data['action'] == 'get_initial_game_data':

		debug.log("Get initial game data", important=True)

		room_pin = json_data['room_pin']

		if room_pin in rooms:
			room = rooms[room_pin]

			room.playfield.cop_start_position = room.playfield.points[0]
			room.playfield.criminal_start_position = room.playfield.points[2]

			cop_target_position_raw = {'longitude': room.playfield.cop_start_position.longitude,
                              'latitude': room.playfield.cop_start_position.latitude}
			criminal_target_position_raw = {'longitude': room.playfield.criminal_start_position.longitude,
                                   'latitude': room.playfield.criminal_start_position.latitude}

			return {'status': 'success', 'cop_target_position': cop_target_position_raw, 'criminal_target_position': criminal_target_position_raw}
		else:
			return {'status': 'failed'}

	elif json_data['action'] == 'update_game_data':

		debug.log("Update game data", important=True)

		room_pin = json_data['room_pin']

		if room_pin in rooms:
			room = rooms[room_pin]

			if json_data['caught'] == True:
				room.caught = True

			player_ready = False
			# Check if the game can start
			if room.game_started == False:
				
				game_can_start = True
				



				for i, player in enumerate(room.playerlist):
					if player.ip == json_data['ip'] and player.name == json_data['name']:

						# Check if player is ready
						target_position = Point(0, 0)
						if player.playertype == PlayerType.cop:
							target_position = room.playfield.cop_start_position

						if player.playertype == PlayerType.criminal:
							target_position = room.playfield.criminal_start_position

						current_position = Point(
							json_data['position']['longitude'], json_data['position']['latitude'])
						player.position = current_position

						#distance = math.sqrt((target_position.longitude - current_position.longitude)**2 + (target_position.latitude - current_position.latitude)**2)
						first_coord = (current_position.latitude, current_position.longitude)
						second_coord = (target_position.latitude, target_position.longitude)
						distance = geopy.distance.distance(first_coord, second_coord).m

						debug.log(str(distance))

						if (distance < room.playfield.max_distance):
							player.is_ready = True
							player_ready = True

					if player.is_ready == False:
						game_can_start = False

				if game_can_start:
					room.game_started = True
					room.game_start_timestamp = time.time()

			time_left = room.game_time-(time.time() - room.game_start_timestamp)

			playerlist = room.playerlist
			playerlistRaw = []
			for player in playerlist:

				playerlistRaw.append(
					{'ip': player.ip, 'name': player.name, 'position': {'longitude': player.position.longitude, 'latitude': player.position.latitude}})

			return {'status': 'success', 'ready': player_ready, 'game_started': room.game_started, 'playerlist': playerlistRaw, 'caught': room.caught, 'time_left': time_left, 'stopped': room.stopped}
		else:
			return {'status': 'failed'}
	elif json_data['action'] == 'exit_game':

		debug.log("Exit game", important=True)

		room_pin = json_data['room_pin']

		if room_pin in rooms:
			room = rooms[room_pin]

			for i, player in enumerate(room.playerlist):
					if player.ip == json_data['ip'] and player.name == json_data['name']:

						if player.playertype == PlayerType.criminal:
							room.stopped = True
							
							return {'status': 'success'}
							
						elif player.playertype == PlayerType.cop:

							if len(room.playerlist) <= 2:
								room.stopped = True
								

								return {'status': 'success'}
							else:
								del room.playerlist[i]
								return {'status': 'success'}
			
			return {'status': 'failed'}
		return {'status': 'failed'}

	else:
		return {'status': 'action does not exist'}


def get_client_address(environ):
    try:
        return environ['HTTP_X_FORWARDED_FOR'].split(',')[-1].strip()
    except KeyError:
        return environ['REMOTE_ADDR']


def application(environ, start_response):

    try:
        request_body_size = int(environ.get('CONTENT_LENGTH', 0))
    except (ValueError):
        request_body_size = 0

    request_body = environ['wsgi.input'].read(request_body_size)
    json_data = json.loads(request_body.decode())
    json_data.update({'ip': get_client_address(environ)})

    debug.log("Incoming json data: "+str(json_data), True)

    outgoing_json_data = json.dumps(handle_json(json_data)).encode()
    debug.log("Outgoing json_data: "+str(outgoing_json_data), True)
    response_body = outgoing_json_data

    status = '200 OK'

    response_headers = [
        ('Content-Type', 'application/json'),
        ('Content-Length', str(len(response_body)))
    ]

    start_response(status, response_headers)
    return [response_body]
