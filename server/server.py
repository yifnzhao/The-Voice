# The Voice : Main server code for NLP and text responses.
# Author :  Korhan Akcura

import json
import requests
import logging

from logging.handlers import RotatingFileHandler

from flask import Flask, jsonify, request

app = Flask(__name__)

@app.route("/")
def root(): 
	return jsonify({"response" : "Hello Word!"})

@app.route('/listen', methods=['POST'])
def listen():
	query = json.loads(request.data)
	response = {
		"received" : query,
		"response" : "I am the chat bot!"
	}
	return jsonify(response)  

if __name__ == '__main__':
	handler = RotatingFileHandler('server.log', maxBytes=10000, backupCount=1)
	handler.setLevel(logging.INFO)
	app.logger.addHandler(handler)
	app.run(host="0.0.0.0",port=5000)

