1.EVENT ORGANISER ENDPOINTS



|HTTP Method|Route|Description|Role Required|Request Body|Expected Response|
|-|-|-|-|-|-|
|GET|/api/organisers|Retrieves a list of registered event organisers.|None|None|200 OK – Returns a list of organisers.|
|GET|/api/organisers/{id}|Retrieves the profile and event details of a specific organiser.|None|None|200 OK – Returns organiser details. 404 Not Found – Organiser does not exist.|
|POST|/api/organisers<br />Create|Creates a new event organiser account.|None|{ "firstName": "John", "lastName": "Smith", "email": "john@email.com", "password": "Password123!|201 Created – Organiser created successfully. 400 Bad Request – Invalid organiser information. 409 Conflict – Email already registered.|
|PUT|/api/organisers/{id}|Updates the details of an existing event organiser.|Organiser|{ "firstName": "John", "lastName": "Smith", "email": "john.smith@email.com", "phone": "0821234567" }|200 OK – Organiser updated successfully. 400 Bad Request – Invalid information. 401 Unauthorized – User is not logged in. 403 Forbidden – User is not authorised to update this profile. 404 Not Found – Organiser does not exist.|
|DELETE|/api/organisers/{id}|Deletes an organiser account from the system.|Organiser|None|204 No Content – Organiser deleted successfully. 401 Unauthorized – User is not logged in. 403 Forbidden – User is not authorised. 404 Not Found – Organiser does not exist.|
|||||||









2.CATEGORY ENDPOINTS



|HTTP Method|Route|Description|Role Required|Request Body|Expected Response|
|-|-|-|-|-|-|
|GET|/api/categories|Retrieves all event categories available in the system.|None|None|200 OK – Returns a list of categories.|
|GET|/api/categories/{id}|Retrieves the details of a specific event category.|None|None|200 OK – Returns category details. 404 Not Found – Category does not exist.|
|POST|/api/categories|Creates a new age, distance, or event category.|Event Organiser|{ "name": "Senior", "minimumAge": 18, "maximumAge": 39, "distance": 10 }|201 Created – Category created successfully. 400 Bad Request – Invalid category information. 401 Unauthorized – User is not logged in. 403 Forbidden – User is not an event organiser.|
|PUT|/api/categories/{id}|Updates an existing event category.|Event Organiser|{ "name": "Senior 10km", "minimumAge": 18, "maximumAge": 39, "distance": 10 }|200 OK – Category updated successfully. 400 Bad Request – Invalid information. 403 Forbidden – User is not authorised. 404 Not Found – Category does not exist.|
|DELETE|/api/categories/{id}|Deletes an event category.|Event Organiser|None|204 No Content – Category deleted successfully. 403 Forbidden – User is not authorised. 404 Not Found – Category does not exist.|







3.PARTICIPANT ENDPOINTS



|HTTP Method|Route|Description|Role Required|Request Body|Expected Response|
|-|-|-|-|-|-|
|GET|/api/participants|Retrieves a list of registered participants.|Event Organiser|None|200 OK – Returns a list of participants. 401 Unauthorized – User is not logged in. 403 Forbidden – User is not an event organiser.|
|GET|/api/participants/{id}|Retrieves the profile and participation details of a specific participant.|Participant or Event Organiser|None|200 OK – Returns participant details. 401 Unauthorized – User is not logged in. 403 Forbidden – User is not authorised. 404 Not Found – Participant does not exist.|
|POST|/api/participants|Registers a new participant account.|None|{ "firstName": "Jane", "lastName": "Doe", "email": "jane@email.com", "password": "Password123!", "dateOfBirth": "2001-05-12" }|201 Created – Participant created successfully. 400 Bad Request – Invalid participant information. 409 Conflict – Email already registered.|
|PUT|/api/participants/{id}|Updates a participant's personal information.|Participant|{ "firstName": "Jane", "lastName": "Doe", "email": "jane.doe@email.com", "phone": "0831234567" }|200 OK – Participant updated successfully. 400 Bad Request – Invalid information. 401 Unauthorized – User is not logged in. 403 Forbidden – User is not authorised to update this profile. 404 Not Found – Participant does not exist.|
|DELETE|/api/participants/{id}|Deletes a participant account.|Participant or Event Organiser|None|204 No Content – Participant deleted successfully. 401 Unauthorized – User is not logged in. 403 Forbidden – User is not authorised. 404 Not Found – Participant does not exist.|







4.WEATHER ENDPOINTS



|HTTP Method|Route|Description|Role Required|Request Body|Expected Response|
|-|-|-|-|-|-|
|GET|/api/weather|Retrieves current weather information for an event location.|None|None|200 OK – Returns current weather information. 400 Bad Request – Invalid location or date. 503 Service Unavailable – Weather service is unavailable.|
|GET|/api/weather/forecast|Retrieves a weather forecast for a selected event date and location.|None|None|200 OK – Returns the weather forecast. 400 Bad Request – Invalid forecast request. 404 Not Found – Forecast information is unavailable. 503 Service Unavailable – External weather service is unavailable.|
|POST|/api/weather/alerts|Creates a weather alert for an event when dangerous conditions are expected.|Event Organiser|{ "eventId": 1, "alertType": "Heavy Rain", "message": "Heavy rain is expected during the event." }|201 Created – Weather alert created successfully. 400 Bad Request – Invalid alert information. 401 Unauthorized – User is not logged in. 403 Forbidden – User is not an event organiser. 404 Not Found – Event does not exist.|
|||||||









5.EVENT ENDPOINTS

|HTTP Method|Route|Description|Role Required|Request Body|Expected Response|
|-|-|-|-|-|-|
|GET|/api/events|Retrieves all upcoming events that users can browse.|None|None|200 OK – Returns a list of events.|
|GET|/api/events/{id}|Retrieves the complete details of a specific event.|None|None|200 OK – Returns event details. 404 Not Found – Event does not exist.|
|POST|/api/events|Creates a new running, walking, or cycling event.|Event Organiser|{ "name": "Durban City Run", "description": "Annual road running event", "date": "2026-10-18", "location": "Durban", "distance": 21, "eventType": "Run", "organiserId": 1 }|201 Created – Event created successfully. 400 Bad Request – Invalid event information. 401 Unauthorized – User is not logged in. 403 Forbidden – User is not an event organiser.|
|PUT|/api/events/{id}|Updates the details of an event created by an organiser.|Event Organiser|{ "name": "Durban City Run", "description": "Updated event description", "date": "2026-10-18", "location": "Durban", "distance": 21, "eventType": "Run" }|200 OK – Event updated successfully. 400 Bad Request – Invalid information. 403 Forbidden – User is not authorised. 404 Not Found – Event does not exist.|
|DELETE|/api/events/{id}|Deletes an event from the system.|Event Organiser|None|204 No Content – Event deleted successfully. 401 Unauthorized – User is not logged in. 403 Forbidden – User is not an event organiser. 404 Not Found – Event does not exist.|



6.REGISTRATION ENDPOINTS



|HTTP Method|Route|Description|Role Required|Request Body|Expected Response|
|-|-|-|-|-|-|
|GET|/api/registrations|Retrieves all event registrations.|Event Organiser|None|200 OK – Returns all registrations. 401 Unauthorized – User is not logged in. 403 Forbidden – User is not an event organiser.|
|GET|/api/registrations/{id}|Retrieves the details of a specific registration.|Participant or Event Organiser|None|200 OK – Returns registration details. 401 Unauthorized – User is not logged in. 403 Forbidden – User is not authorised. 404 Not Found – Registration does not exist.|
|POST|/api/registrations|Registers a participant for an event and selected category.|Participant|{ "participantId": 1, "eventId": 1, "categoryId": 2 }|201 Created – Registration completed successfully. 400 Bad Request – Invalid registration information. 401 Unauthorized – User is not logged in. 403 Forbidden – User is not a participant. 404 Not Found – Event or category does not exist. 409 Conflict – Participant is already registered for the event.|
|PUT|/api/registrations/{id}|Updates an existing registration,such as changing the selevted category.|Participant or Event Organiser|{ "categoryId": 3, "status": "Confirmed" }|200 OK – Registration updated successfully. 400 Bad Request – Invalid information. 401 Unauthorized – User is not logged in. 403 Forbidden – User is not authorised. 404 Not Found – Registration does not exist.|
|DELETE|/api/registrations/{id}|Cancels or removes a participant's event registration.|Participant or Event Organiser|None|204 No Content – Registration cancelled successfully. 401 Unauthorized – User is not logged in. 403 Forbidden – User is not authorised. 404 Not Found – Registration does not exist.|





7.RESULTS ENDPOINTS



|HTTP Method|Route|Description|Role Required|Request Body|Expected Response|
|-|-|-|-|-|-|
|GET|/api/results|Retrieves all published event results.|None|None|200 OK – Returns a list of published results.|
|GET|/api/results/{id}|Retrieves the details of a specific result.|Participant or Event Organiser|None|200 OK – Returns result details. 401 Unauthorized – User is not logged in. 403 Forbidden – User is not authorised to view this result. 404 Not Found – Result does not exist.|
|GET|/api/results/my|Retrieves the authenticated participant's personal race results and history.|Participant|None|200 OK – Returns the participant's results. 401 Unauthorized – User is not logged in. 403 Forbidden – User is not a participant.|
|POST|/api/results|Records a participant's finishing time and position after an event.|Event Organiser|{ "registrationId": 1, "finishTime": "01:42:35", "position": 47 }|201 Created – Result recorded successfully. 400 Bad Request – Invalid result information. 401 Unauthorized – User is not logged in. 403 Forbidden – User is not an event organiser. 404 Not Found – Registration does not exist. 409 Conflict – A result already exists for this registration.|
|PUT|/api/results/{id}|Updates an existing participant result.|Event Organiser|{ "finishTime": "01:41:52", "position": 45 }|200 OK – Result updated successfully. 400 Bad Request – Invalid result information. 401 Unauthorized – User is not logged in. 403 Forbidden – User is not an event organiser. 404 Not Found – Result does not exist.|
|DELETE|/api/results/{id}|Removes an incorrect or invalid result from the system.|Event Organiser|None|204 No Content – Result deleted successfully. 401 Unauthorized – User is not logged in. 403 Forbidden – User is not an event organiser. 404 Not Found – Result does not exist.|







8.AUTHENTICATION ENDPOINTS



|HTTP Method|Route|Description|Role Required|Request Body|Expected Response|
|-|-|-|-|-|-|
|POST|/api/auth/register|Registers a new user as either a Participant or Event Organiser.|None|{ "firstName": "John", "lastName": "Smith", "email": "john@email.com", "password": "Password123!", "role": "Participant" }|201 Created – Account created successfully. 400 Bad Request – Invalid registration information. 409 Conflict – Email already registered.|
|POST|/api/auth/login|Authenticates a user and creates a secure session.|None|{ "email": "john@email.com", "password": "Password123!" }|200 OK – Login successful. 400 Bad Request – Missing or invalid fields. 401 Unauthorized – Incorrect email or password.|
|POST|/api/auth/logout|Ends the current user's authenticated session.|Participant or Event Organiser|None|204 No Content – Logout successful. 401 Unauthorized – User is not logged in.|
|||||||





9.USER PROFILE ENDPOINTS



|HTTP Method|Route|Description|Role Required|Request Body|Expected Response|
|-|-|-|-|-|-|
|GET|/api/profile|Retrieves the profile of the currently authenticated user.|Participant or Event Organiser|None|200 OK – Returns the user's profile. 401 Unauthorized – User is not logged in. 404 Not Found – Profile does not exist.|
|PUT|/api/profile|Updates the personal information of the currently authenticated user.|Participant or Event Organiser|{ "firstName": "John", "lastName": "Smith", "email": "john.smith@email.com", "phone": "0821234567" }|200 OK – Profile updated successfully. 400 Bad Request – Invalid information. 401 Unauthorized – User is not logged in. 409 Conflict – Email already in use.|
|PUT|/api/profile/password|Changes the password of the currently authenticated user.|Participant or Event Organiser|{ "currentPassword": "Password123!", "newPassword": "NewPassword456!" }|200 OK – Password changed successfully. 400 Bad Request – Invalid password information. 401 Unauthorized – Current password is incorrect or user is not logged in.|
|||||||







10.EVENT ENROLMENT ENDPOINTS



|HTTP Method|Route|Description|Role Required|Request Body|Expected Response|
|-|-|-|-|-|-|
|POST|/api/enrolments|Enrols the authenticated participant in an event by selecting an available category.|Participant|{ "eventId": 1, "categoryId": 2 }|201 Created – Enrolment recorded successfully. 400 Bad Request – Invalid enrolment information. 401 Unauthorized – User is not logged in. 403 Forbidden – User is not a participant. 404 Not Found – Event or category does not exist. 409 Conflict – Participant is already enrolled.|
|GET|/api/events/{eventId}/enrolments|Retrieves all participants enrolled in an event managed by the organiser.|Event Organiser|None|200 OK – Returns all enrolments for the event. 401 Unauthorized – User is not logged in. 403 Forbidden – User is not an event organiser. 404 Not Found – Event does not exist.|
|DELETE|/api/enrolments/{id}|Cancels a participant's enrolment in an event.|Participant or Event Organiser|None|204 No Content – Enrolment cancelled successfully. 401 Unauthorized – User is not logged in. 403 Forbidden – User is not authorised. 404 Not Found – Enrolment does not exist.|
|||||||
|||||||



















































