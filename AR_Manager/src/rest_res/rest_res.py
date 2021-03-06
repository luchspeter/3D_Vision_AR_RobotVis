import sys
from flask_restful import Resource, reqparse

class ResInstances(Resource):
	def __init__(self, server):
		self.s = server

	def get(self):

		data = self.s.get_instances()
		print(self.s)
		return data

	def post(self):

		parser = reqparse.RequestParser()
		parser.add_argument('comp_name', type=str, required=True, location='json')
		args = parser.parse_args()
		try:
			res = self.s.start(args['comp_name'])
		except ValueError:
			res = -1

		print(self.s)
		return res

	def delete(self):
		self.s.stop_instances()

		print(self.s)
		return {'Suc':True}
	
class ResInstance(Resource):
	def __init__(self, server):

		self.s = server

	def get(self,inst_id):
		print(self.s)
		try:
			return self.s.get_instance(inst_id)
		except ValueError:
			return -1
			 
	def post(self,inst_id):

		parser = reqparse.RequestParser()
		parser.add_argument('comp_name', type=list, required=True, location='json')
		args = parser.parse_args()

		try:
			res = self.s.start(args['comp_name'])
		except ValueError:
			res = -1

		print(self.s)
		return res

	def delete(self,inst_id):
		print(inst_id, type(inst_id))
		try:
			res = self.s.stop_instance(inst_id)
		except ValueError:
			res = -1
			
		print(self.s)
		return res

class ResInstanceUrdfDyn(Resource):
	def __init__(self, server):

		self.s = server

	def post(self,inst_id):

		parser = reqparse.RequestParser()
		parser.add_argument('data', type=str, required=True, location='json')
		args = parser.parse_args()
		res = self.s.update_instance_urdf(inst_id ,args['data'])
		print(self.s)
		return res

class ResAvailComps(Resource):

	def __init__(self, server):
		
		self.s = server

	def get(self):
		
		res = self.s.get_avail_comps()
		print(self.s)
		return res

	def post(self):

		parser = reqparse.RequestParser()
		parser.add_argument('components', type=list, required=True, location='json')
		args = parser.parse_args()

		self.s.add_comps(args['components'])
		res = self.s.get_avail_comps() 
		print(self.s)
		return res


class ResAvailComp(Resource):

	def __init__(self, server):
		self.s = server

	def get(self,name):
		print(self.s)
		try: 
			res = self.s.get_avail_comp(name)
		except ValueError:
			res = -1
		return res

	def delete(self,name):
		try:
			res = self.s.remove_avail_comp(name)
		except ValueError:
			res = -1
		print(self.s)
		return res