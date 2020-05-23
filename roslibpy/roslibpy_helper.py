import roslibpy
import time
from scipy.spatial.transform import Rotation as R
import yaml 


ip ='http://vservice-host-001.chera.ch' #127.0.0.1
ip_ros ='vservice-host-001.chera.ch' 
port_rest = 5000
port_rossharp = 9090

ros = roslibpy.Ros(host=ip_ros, port= port_rossharp)
ros.run()
print(ros.get_topics())

tp_name ='/joint_states'
tp_type = ros.get_topic_type(tp_name)
print(tp_type, tp_name)

# listener = roslibpy.Topic(ros, tp_name, tp_type )
# listener.subscribe(lambda message: print('\n \n Message:' +str(message) ) )

def create_Pose(t,xyz):
	q = R.from_euler('xyz' , xyz).as_quat()
	return { 'position': {'x':t[0], 'y':t[1], 'z':t[2]} , 'orientation':{'x':q[0] ,'y':q[1],'z':q[2],'w':q[3] } }

def create_ModelState(data):

	model_name = 'robot'
	pose = create_Pose([0,0,0],[0,0,0.1])
	twist = {'linear':{'x':0,'y':0,'z':0}, 'angular':{'x':0,'y':0,'z':0}}
	reference_frame = 'base_link'
	return {
		'model_name':model_name,
		'pose':pose,
		'twist':twist,
		'reference_frame':reference_frame
	}

talker_gazebo_reset = roslibpy.Topic(ros, '/gazebo/set_model_state', 'gazebo_msgs/ModelState')

link_top = '/gazebo/set_link_state'
talker_gazebo_link_reset = roslibpy.Topic(ros, link_top , ros.get_topic_type(link_top) )

with open('link_states.yaml') as f:
			msg = yaml.load(f, Loader=yaml.FullLoader)


srv = '/gazebo/set_link_state'
serv = roslibpy.Service(ros, srv, 'gazebo_msgs/SetLinkState')
def callback(data):
	print("got call")
	print("i get data back" ,data)

while ros.is_connected:
	#print(msg)
	for i,n in enumerate(msg['name']):
		ref = n.split(':')[0]
		nam = n #.split(':')[-1]
		msg2 = {'link_name':nam, 'pose':msg['pose'][i] ,'twist':msg['twist'][i] ,'reference_frame':'world'}
		#print(msg2, '\n \n \n')
		request = roslibpy.ServiceRequest()
		#print('send')
		talker_gazebo_link_reset.publish( roslibpy.Message( msg2 ))
		serv.call( request,callback )
		#print('try')
		#talker_gazebo_link_reset.publish(roslibpy.Message( msg2 ) )
		#talker_gazebo_reset.publish(roslibpy.Message( create_ModelState(0) ) )
	
	
	#print("sending")
	time.sleep(0.1)