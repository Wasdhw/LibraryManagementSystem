
CREATE TABLE books (

	id INT PRIMARY KEY identity(1,1),
	book_title VARCHAR(max) NULL,
	author VARCHAR(max) NULL,
	published_date VARCHAR(max) NULL,
	quantity INT NULL,
	image VARCHAR(max) NULL,
	status VARCHAR(max) NULL,
	date_insert DATE NULL,
	date_update DATE NULL,
	date_delete DATE NULL

);
	

select * from users

select * from books