# OAuth2ValidationFromScratchWithDI


This application focuses on authenticating Google cloud API using OAuth2.0 bearer token. Authenticating this API consists of mainly 3 parts:

1. Creating our own JWT with credentials, signature, scopes, etc.
2. Hitting Google's API with that JWT token in header to get bearer token in return.
3. If bearer token is returned:
      Hitting prediction API with that bearer token in request header, which will return the expected result in JSON format.
      
      
( Note: Google cloud's SDK can be used to simplify this overall process)

In addition to that, this application brings dependency injection in console application. This approach has following steps:

1. Creating interfaces and base class
2. Creating services from CollectionServices class and registering and disposing those services.
3. Inject those service in ConsoleApplication.cs class

Happy Coding !!!

