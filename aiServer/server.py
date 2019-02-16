#----------------------------------------------------------------------
#  server.py
#
# The Voice : Main server code for NLP and text responses.
# Author :  Korhan Akcura
#----------------------------------------------------------------------
import json
import requests
import logging
#logging.basicConfig(level=logging.CRITICAL)
from func_timeout import func_timeout
from models.eliza import eliza
from models.chatterbot import chatterbot_facade
from stubs.emotion import emotion

from logging.handlers import RotatingFileHandler

from flask import Flask, jsonify, request

app = Flask(__name__)

# Response bots.
eliza_bot = eliza.eliza()
chatter_bot = chatterbot_facade.chatterbot_facade()
emotion_bot = emotion.emotion()

@app.route("/", methods=['GET', 'POST'])
def root(): 
	return jsonify({"response" : "Hello Word!"})

@app.route('/listen', methods=['POST'])
def listen():
	try:
		json_response = json.loads(request.get_data().decode('utf8'))
		print("Received: " + str(json_response))
		query = json_response['content']
		pitch = float(json_response['pitch'])
	except Exception:
		return jsonify({"response" : "Bad request!"}), 400

	# Default response.
	response = "I could not understand!"

	# Smart response.
	try:
		# Chatterbot response.
		# This will time out and throw exception
		# if an answer is not generated.
		response = chatter_bot.respond(query)
		print("ChatterBot Responding.")
	except Exception:
		# Fall-back response
		response = eliza_bot.respond(query)
		print("Eliza Responding.")

	# Detect emotion.
	emotion_paramaters = emotion_bot.predict(query,pitch)

	response = {
		"response"   : response,
		"emotion"    : emotion_paramaters["emotion"],
		"confidence" : emotion_paramaters["confidence"]
	}
	print("Final Response: " + str(response))
	return jsonify(response)  

if __name__ == '__main__':
	handler = RotatingFileHandler('server.log', maxBytes=10000, backupCount=1)
	handler.setLevel(logging.INFO)
	app.logger.addHandler(handler)
	app.run(host="127.0.0.1",port=5000)

