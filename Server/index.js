
var tfserver = require('tfserver');

tfserver.Server({
	// Server Settings
	'port': 4242,
	'json': '../Assets/Resources/rpcs.json'
}, function(err, server) {
	server.on('SendMessage', function(data, callback) {
		console.log(data);
		callback(data);
	});
	server.on('UpdatePosition', function(data, callback) {
		console.log(data);
		callback(data);
	});
});
