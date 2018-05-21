# `Tethys` Mock REST Server for Rest client tests 
`Tethys` created due to the demand for testing the behavior of rest client when interacting with the server side.
Basically it is smart mock server that enables the developer to setup a sequences of expected HTTP request that would be sent to the REST server and the corrosponding HTTP responses `Tethys` would send back.
When HTTP request reach the server, `Tethys` compares the incoming request's data to the expected value and return the required response.

## REST client-server interaction pattern
REST defines 2 main streams of client-server interaction;
- Client initiates HTTP request calls directed to the server (and recieves answer in the form of HTTP response)
- Server send notification (via websocket) to client. Client recieves this notifications in async manner.

## How `Tethys` uses this pattern for testing?
`Tethys` creates mock server instance that simulates the rest server of your system, examines the incoming requests and sends back the required responses.

