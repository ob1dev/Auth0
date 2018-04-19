# Auth0-Coding-Project

## Problem Overview

Auth0 allows customers to create tokens to protect their own APIs (https://auth0.com/docs/api-auth) using standard OAuth2 flows. As part of the configuration Auth0 allows to define a know lists of scopes available for APIs that might be used to display consent forms and it allows developers to modify the scopes that will be included in the resulting tokens. 

## Requirements

Provide a solution that allows a customer to define limited access to their API:

1. The user authenticates using Auth0
2. Based on the user job title the user may have access to
   - Read Data
   - Write Data or
   - Delete Data
3. The application gets a token to act on behalf of the user
4. The application calls the API to perform any activities

Build a sample application (SPA or Web Application) and a dummy service that depict the flow required to implement the authentication and the API Call.

## Extra credit

- Do not use the `Authorization Extension`.
- Enable usage of `refresh_token` for mobile applications.
- Add your own custom rule (not from a template) that enriches the user profile.

Please share the code on GitHub and make sure the README file is clean and clear so customers can understand it.