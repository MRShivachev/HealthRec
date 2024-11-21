# HealthRec

## Overview

HealthRec is a web application designed to help healthcare professionals efficiently manage and access patient information. This project allows users to list, sort, and access different hospital patients' records, ensuring that vital information is readily available when needed. Built using C#, HealthRec aims to streamline patient data management, improve workflow efficiency, and enhance the quality of care provided to patients.

## Features

- **Patient Listing**: View a comprehensive list of all patients in the hospital.
- **Sorting Options**: Sort patient records by various criteria such as name, admission date, or medical condition.
- **Search Functionality**: Quickly find specific patient records using a search bar.
- **Detailed Patient View**: Access detailed information for each patient, including medical history, current medications, and treatment plans.
- **User  Authentication**: Secure login for healthcare professionals to protect sensitive patient information.
- **Responsive Design**: A user-friendly interface that works seamlessly on both desktop and mobile devices.

## Technologies Used

- **Programming Language**: C#
- **Framework**: ASP.NET Core
- **Database**: SQL Server
- **Frontend**: HTML, CSS, JavaScript (with optional frameworks like Bootstrap)
- **Version Control**: Git

## Installation

To set up the HealthRec project locally, follow these steps:

1. **Clone the Repository**:
   ```bash
   git clone https://github.com/yourusername/HealthRec.git
2. **Navigate to the Project Directory:**:
   ```bash
   cd HealthRec
3. **Install Dependencies:** Make sure you have the .NET SDK installed. Run the following command to restore the required packages:

   ```bash
   dotnet restore
4. **Set Up the Database:**
- Configure your database connection string in the `appsettings.json` file.
- Run the migrations to set up the database schema:

   ```bash
   dotnet ef database update
5. **Run the Application:** Start the application using the following command:

   ```bash 
   dotnet run
6. **Access the App:** Open your web browser and navigate to `http://localhost:5000` to access the HealthRec application.

## Usage

- **Login**: Use your credentials to log in to the application.
- **View Patients**: Navigate to the "Patients" section to view the list of patients.
- **Sort and Filter**: Use the sorting and filtering options to find specific patient records.
- **View Details**: Click on a patient's name to view their detailed information.

---

Thank you for checking out HealthRec! We hope this application helps improve patient information management in healthcare settings.