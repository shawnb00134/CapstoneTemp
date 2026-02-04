# Getting Started with CAM CMS React App

This project was bootstrapped with [Create React App](https://github.com/facebook/create-react-app).

It uses [JavaScript](https://devdocs.io/javascript/) and [React](https://reactjs.org/) framework.

## Setup and Launching
- Setup: Run ..\project-setup.ps1
- Launch: Run ..\launch-web.sh

## Style Guide <!-- TODO: Check with team -->

### [React](https://reactjs.org/docs/getting-started.html)
- Use functional components over class components.
- Use hooks over class components.
- Use function declarations over lambda functions.
- Use JavaScript over TypeScript.

### [JavaScript](https://devdocs.io/javascript/)
- Use [ES6](https://devdocs.io/javascript/) syntax.
- Use [ESLint](#eslint) for linting.
- Use `var` and `const` over `let`.
- Page files should be named using [CamelCase](https://en.wikipedia.org/wiki/CamelCase)
- Component files should be named using [CamelCase](https://en.wikipedia.org/wiki/CamelCase)
- Code should be indented using tabs.
- Code should be formatted using [EsLint](#eslint) for Javascript and [Stylelint](#stylelint) for CSS.
- Document code using [JSDoc](https://devdocs.io/jsdoc/).

### [CSS](https://devdocs.io/css/)
- Use [Stylelint](#stylelint) for linting.
- Use [Classic CSS](https://devdocs.io/css/) for naming classes.

### [HTML](https://devdocs.io/html/)
- Use [HTML5](https://devdocs.io/html/) syntax.

## Available Scripts

Before running the below scripts, it is recommended you run:

### `npm install`

To ensure the environment is confirgured correctly for running the app.

In the this directory, you can run:

### `npm start`

Runs the app in the development mode on port `3000` by default.\
Open [http://localhost:3000](http://localhost:3000) to view it in your browser if ran local.

The page will reload when you make changes.\
You may also see any lint errors in the console.

Run `HTTPS=true npm start` to the start the app in the development mode with HTTPS on linux.\
To do so on Windows, use set `HTTPS=true&&npm start` instead.

Flags:
 - #### `HTTPS=true`
    **Note: `HTTPS=true&&npm start` on Windows and `HTTPS=true npm start` on Linux**\
    Runs the app in the development mode with HTTPS.

 - #### `--port 3006`
    Runs the app in the development mode on port `3006` instead of the default port `3000`.

#### `npm run start:dev`

Runs the app in the development mode on port `3000` by default with .env.development.\
Open [http://localhost:3000](http://localhost:3000) to view it in your browser if ran local.

The page will reload when you make changes.\
You may also see any lint errors in the console.

#### `npm run start:prod`

Runs the app in the production mode on port `3000` by default with .env.production.\
Open [http://localhost:3000](http://localhost:3000) to view it in your browser if ran local.

The page will reload when you make changes.\
You may also see any lint errors in the console.

### Linting

#### `.\linting.bat`
**Note: This has only been tested on Windows 10 currently**

This will run the both the stylelint and eslint linters on the src folder and report\
any errors or warnings in any CSS and JS files.

This will also use the `--fix` flag to automatically fix any errors possible.

#### `npx stylelint src`
**Note: Add the `--fix` flag to the end of the command to automatically fix any errors.**

This will run the stylelint linter on the src folder and report\
any errors or warnings in any JS files.

#### `npx eslint src`
**Note: Add the `--fix` flag to the end of the command to automatically fix any errors.**

This will run the eslint linter on the src folder and report\
any errors or warnings in any CSS files.

### `npm run build`

Builds the app for production to the `build` folder.\
It correctly bundles React in production mode and optimizes the build for the best performance.

The build is minified and the filenames include the hashes.\
Your app is ready to be deployed!

See the section about [deployment](https://facebook.github.io/create-react-app/docs/deployment) for more information.

#### `npm run build:dev`

Builds the app for development to the `build` folder with .env.development.\
It correctly bundles React in development mode and optimizes the build for the best performance.

#### `npm run build:prod`

Builds the app for production to the `build` folder with .env.production.\
It correctly bundles React in production mode and optimizes the build for the best performance.


## Learn More

You can learn more in the [Create React App documentation](https://facebook.github.io/create-react-app/docs/getting-started).

To learn React, check out the [React documentation](https://reactjs.org/).

To learn more about other libraries used in this project, take a look at the following sections.

## Third Party Libraries

### [TinyMCE](https://www.tiny.cloud/)

TinyMCE is a rich text editor that allows users to edit text in a web browser. It is a WYSIWYG editor, which means that the text being edited on it looks as similar as possible to the results users have when publishing it.

For more information on TinyMCE, check out the [TinyMCE documentation](https://www.tiny.cloud/docs/).

### [Font Awesome](https://fontawesome.com/)

Font Awesome is a font and icon toolkit based on CSS and LESS. It was made by Dave Gandy for use with Twitter Bootstrap, and later was incorporated into the BootstrapCDN.

For more information on Font Awesome, check out the [Font Awesome documentation](https://fontawesome.com/how-to-use/on-the-web/using-with/react).

### [Eslint](https://eslint.org/)

ESLint is a static code analysis tool for identifying problematic patterns found in JavaScript code. It was created by Nicholas C. Zakas in 2013. Rules in ESLint are configurable, and customized rules can be defined and loaded. ESLint covers both code quality and coding style issues.

For more information on Eslint, check out the [Eslint documentation](https://eslint.org/docs/user-guide/getting-started).

### [Stylelint](https://stylelint.io/)

Stylelint is a linter that helps you avoid errors and enforce conventions in your styles. It is powered by plugins, including rules that are community maintained and can be shared and reused.

For more information on Stylelint, check out the [Stylelint documentation](https://stylelint.io/user-guide/get-started).

### [Cypress](https://www.cypress.io/)

Cypress is a next generation front end testing tool built for the modern web. Cypress is designed for E2E testing and is used to test anything that runs in a browser. It allows for fast, easy an reliable testing for UI elements and interactions.

For more information on Cypress, check out the [Cypress documentation](https://docs.cypress.io/guides/overview/why-cypress.html#In-a-nutshell).
