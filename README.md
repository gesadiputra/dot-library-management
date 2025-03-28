API User:<br>
POST /User/login - Login user<br>
GET /User/me - Get data user logged in<br>
POST /User/superadmin - Create a Superadmin user<br>
POST /User - Create a new user<br>
PUT /User - Update existing user<br>
DELETE /User - Delete existing user<br>
GET /User - Get List of user<br>
GET /User/{userId} - Get user detail<br>
<br>
Api Book:<br>
POST /Book - Create new book<br>
PUT /Book - Update existing book<br>
DELETE /Book - Delete existing book<br>
GET /Book - Get list of book<br>
GET /Book/{bookId} - get book detail<br>
GET /Book/author - get list of book author (for create & update book)<br>
GET /Book/genre - get list of genre (for create & update book)<br>
<br>
In API Create & Update there are two redundant body:<br>
"author" -> string<br>
"authorId" -> guid<br>
For Create and Update, either "author" or "authorId" must be entered, or it will throw exception.<br>
Only one can be used, so if using "author", "authorId" must not be used<br>
If using "author", a new Author will be created and used<br>
If using "authorId", an existing Author will be used<br>
<br>
"genreIds" -> list of guid<br>
"genres" -> list of string<br>
For Create and Update, "genreIds" and "genres" must be entered, or it will throw exception.<br>
Either of them can be used<br>
If using "genreIds", an existing Genre will be used<br>
If using "genres", a new Genre will be created and used<br>
