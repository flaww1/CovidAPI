# COVID-19 Data Analysis and Visualization Project

## Introduction

This project focuses on the comprehensive analysis and visualization of COVID-19 data collected during the pandemic, with a specific emphasis on European countries from 2020 to 2022. The primary goal is to provide a deeper understanding of the pandemic's evolution, integrating geolocation services to enable visual representation on maps.

The project seamlessly integrates Extract, Transform, Load (ETL) tools and demonstrates the practical application of system integration concepts. It utilizes web services, external APIs, and showcases the results through a dashboard, deployed on cloud infrastructure.

## Table of Contents

- [Introduction](#introduction)
- [Process Documentation](#process-documentation)
- [Doxygen Documentation](#doxygen-documentation)
- [Project Overview](#project-overview)
- [ETL (Extract, Transform, Load)](#etl-extract-transform-load)
- [COVID-19 Data Services](#covid-19-data-services)
- [Geolocation Data Services](#geolocation-data-services)
- [Demonstration of COVID-19 and Geolocation Services](#demonstration-of-covid-19-and-geolocation-services)
- [Authentication Services](#authentication-services)
- [Authentication Services Demonstration](#authentication-services-demonstration)
- [Dashboard](#dashboard)
- [Cloud Deployment](#cloud-deployment)
- [Conclusion](#conclusion)

## Process Documentation

The documentation process was executed using Doxygen and Graphviz, generating consistent and well-formatted documentation directly from the source code. The [Doxygen Documentation](#doxygen-documentation) section details the steps involved.

## Doxygen Documentation

The [Doxygen Documentation](#doxygen-documentation) section provides insights into the Doxygen integration process, generating HTML and PDF documentation for the project.

## Project Overview

This section offers a brief overview of the project, highlighting its components, functionalities, and the integration of ETL tools. The [ETL (Extract, Transform, Load)](#etl-extract-transform-load) section dives into the details of the initial data processing steps.

## ETL (Extract, Transform, Load)

Details the initial steps involving the extraction, transformation, and loading of COVID-19 data using CSV datasets. The MySQL database is utilized for data storage, and the ETL process is outlined. See [ETL (Extract, Transform, Load)](#etl-extract-transform-load) for a detailed description.

## COVID-19 Data Services

Explores the creation of services dedicated to handling COVID-19 data. Two models, `CovidData` and `CovidDataDTO`, are introduced, emphasizing the separation of internal data representation and external presentation. The [COVID-19 Data Services](#covid-19-data-services) section delves deeper into these services.

## Geolocation Data Services

Describes the component responsible for geolocation operations and introduces models like `GeolocationAPIResponse` and `GeolocationResult`. The integration of geolocation services with COVID-19 data services is explained. Further details can be found in [Geolocation Data Services](#geolocation-data-services).

## Demonstration of COVID-19 and Geolocation Services

Illustrates practical examples using Postman, showcasing a GET request for COVID-19 data and geolocation information. Sample responses are included in the [Demonstration of COVID-19 and Geolocation Services](#demonstration-of-covid-19-and-geolocation-services) section.

## Authentication Services

Introduces authentication-related models and services, emphasizing security and user authentication. The [Authentication Services](#authentication-services) section provides details on the `AuthService`, `PasswordService`, and `UserService`.

## Authentication Services Demonstration

Demonstrates user registration and login processes using Postman, emphasizing token generation and secure password handling. For more details, refer to [Authentication Services Demonstration](#authentication-services-demonstration).

## Dashboard

Details the creation of a dashboard using React, showcasing interactive maps and charts. The integration of Axios, Leaflet.js, and Recharts is highlighted. Explore the [Dashboard](#dashboard) section for more information.

## Cloud Deployment

Outlines the challenges faced during the Microsoft Azure deployment and provides insights into potential opportunities for future exploration. The [Cloud Deployment](#cloud-deployment) section briefly discusses the deployment process.

## Conclusion

Summarizes the key outcomes of the project, emphasizing the successful integration of tools, documentation efficiency, and practical application of concepts. Future opportunities for improvement are highlighted. For a more detailed conclusion, see [Conclusion](#conclusion).
