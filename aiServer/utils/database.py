#----------------------------------------------------------------------
#  database.py
#
# The Voice : Database util methods.
# Author :  Korhan Akcura
#----------------------------------------------------------------------
from pymongo import MongoClient
from timeout3 import timeout, TIMEOUT_EXCEPTION

class database:
	def __init__(self):
		self.connectionString = "mongodb://localhost"
		self.client = ""
		self.dbnames = ""

	def get_connection_url(self):
		return self.connectionString

	def check_db_exists(self):
		try:			
			return self.check_mongo_exists()
		except Exception:
			return False

	@timeout(1)
	def check_mongo_exists(self):
		self.client = MongoClient(self.connectionString)
		self.dbnames = self.client.list_database_names()
		return True

	#----------------------------------------------------------------------
	#  Check if a database exists.
	#----------------------------------------------------------------------
	def check_mongo_db_exists(self,database):
		if database in self.dbnames:
			return True
		else:
			return False
