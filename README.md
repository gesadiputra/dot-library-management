API User:
POST /User/login - Login user
GET /User/me - Get data user logged in
POST /User/superadmin - Create a Superadmin user
POST /User - Create a new user
PUT /User - Update existing user
DELETE /User - Delete existing user
GET /User - Get List of user
GET /User/{userId} - Get user detail

Api Book:
POST /Book - Create new book
PUT /Book - Update existing book
DELETE /Book - Delete existing book
GET /Book - Get list of book
GET /Book/{bookId} - get book detail
GET /Book/author - get list of book author (for create & update book)
GET /Book/genre - get list of genre (for create & update book)

In API Create & Update there are two redundant body:
"author" -> string
"authorId" -> guid
For Create and Update, either "author" or "authorId" must be entered, or it will throw exception.
Only one can be used, so if using "author", "authorId" must not be used
If using "author", a new Author will be created and used
If using "authorId", an existing Author will be used

"genreIds" -> list of guid
"genres" -> list of string
For Create and Update, "genreIds" and "genres" must be entered, or it will throw exception.
Either of them can be used
If using "genreIds", an existing Genre will be used
If using "genres", a new Genre will be created and used
