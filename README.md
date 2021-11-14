# HospitalManagmentSystem_
> The system is intended to allow users to keep simple records of hospital employees.

## Table of Contents
* [General Info](#general-information)
* [Technologies Used](#technologies-used)
* [Setup](#setup)
* [Contact](#contact)
<!-- * [License](#license) -->


## General Information
The application HospitalManagmentSystem_ is a project at the university.

Project assumptions:
- The aim of the project was to create an administrative system for the hospital. The system is to allow users to keep a simple record of hospital employees. Each employee has     name, surname and pesel as well as username and password. In the system we distinguish the following types of users: doctor, nurse, administrator.
- A doctor, apart from the standard data of each user, has additionally a specialty (cardiologist, urologist, neurologist or laryngologist) and a PWZ number. Doctors and nurses   also have a list of their 24-hour on-call duties, with the assumption that one person can have a maximum of 10 on-call duties per month and their on-call duties cannot occur     day after day. In addition, only one doctor per specialty may be on duty on any given day (e.g., a cardiologist, a urologist and an ENT may be on duty on a given day, but not   two cardiologists).
- When the system starts, it asks you to enter your user name and password. After logging in, in the case of doctors and nurses, you can only see a list of all doctors and         nurses (first name, last name, job title + specialization if any) and the duty roster of the selected person in the given month.
- Administrator after logging in can see all users on the list. He can also edit the data of each user (including duty roster) and add new users (including administrators) to     the system.

## Technologies Used
- .NETFramework - version 4.7.2
- EntityFramework - version 6.4.4
- MaterialDesignColors - version 1.2.7
- MaterialDesignThemes - version 3.2.0


## Sample User
- username: admin
- password: admin


## Contact
Created by [Adam Stelmaszak](www.linkedin.com/in/adam-stelmaszak) - feel free to contact me!
