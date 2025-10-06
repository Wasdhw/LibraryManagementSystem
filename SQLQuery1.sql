CREATE TABLE users (
id INT PRIMARY KEY identity(1,1),
email VARCHAR(max) NULL,
username VARCHAR(max) NULL,
password VARCHAR(max) NULL,
date_register DATE NULL,


);

select * from users