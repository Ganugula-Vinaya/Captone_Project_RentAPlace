# RentAPlace — Frontend

**Frontend** for RentAPlace — Angular single page application (SPA).  
This README explains how to run the app locally, configure the API base URL, debug common issues, build for production, and deploy.

---

## Table of contents

- [Prerequisites](#prerequisites)
- [Quick start (development)](#quick-start-development)
- [Environment configuration](#environment-configuration)
- [Running with the .NET backend](#running-with-the-net-backend)
- [Build for production](#build-for-production)
- [Serve production build locally](#serve-production-build-locally)
- [Testing & linting](#testing--linting)
- [Common issues & troubleshooting](#common-issues--troubleshooting)
- [Useful scripts](#useful-scripts)
- [API endpoints used by frontend (reference)](#api-endpoints-used-by-frontend-reference)


## Prerequisites

- Node.js (v16+ recommended; v18+ preferred)
- npm (comes with Node) or yarn
- Angular CLI (optional for global usage): `npm i -g @angular/cli`
- .NET backend running locally (dev backend used in examples is at `http://localhost:5264`)
- Optional: `http-server` for quick static serving: `npm i -g http-server`

---

## Quick start (development)

1. Unzip the frontend and open a terminal in the project root:
   ```bash
   cd path/to/frontend

2. Install dependencies:
    npm install

3. Register CORS policy in middleware(Program.cs)

4. Start the dev server:

# if package.json has "start" script
npm start

# or directly:
ng serve --open


Open http://localhost:4200 in your browser

## Environment configuration

Angular stores app settings in src/environments/environment.ts and src/environments/environment.prod.ts.

Create or edit src/environments/environment.ts:

export const environment = {
  production: false,
  // set to your backend base URL (no trailing slash)
  apiBase: 'http://localhost:5264'
};