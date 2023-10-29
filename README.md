# Calendar
A simple, self-made calendar app, crafted for personal enjoyment. It provides basic calendar and event functionalities to help you manage your calendar and schedule events effortlessly, without the complexities of mainstream alternatives.

> **Warning**
> This is a work in progress and some features are still missing at the moment.

## Documentation

> **Warning**
> Work in progress.

## Installation

Follow these steps to set up the project on your local machine:

### Prerequisites:

- **Visual Studio**: Ensure you have [Visual Studio](https://visualstudio.microsoft.com/) installed. The latest version is recommended.
  
- **.NET SDK**: Ensure you have the .NET SDK corresponding to the project's version installed. You can check the `.csproj` file for the target framework.

- **Node.js**: As this project uses Vue 3 for the frontend, you'll need [Node.js](https://nodejs.org/) installed. This will come with npm (Node package manager) which is used for managing frontend dependencies.

### Steps:

1. **Clone the Repository**:
   ```sh
   git clone <repository-url>
   cd <repository-name>
   ```

2. **Open in Visual Studio**:
  - Navigate to the location where you cloned the repository.
  - Double-click on the .sln file to open the solution in Visual Studio.
  
3. **Auto Dependency Installation**: 
   - By default, this project is set up to automatically restore .NET dependencies.
   - When you build the project for the first time, Visual Studio should handle .NET dependencies for you.
   - For frontend dependencies, navigate to the directory containing the `package.json` file (usually the root of the frontend project or `ClientApp` folder) and run:
     ```sh
     npm install
     ```

4. **Build the Solution**:
   - In Visual Studio, right-click on the Solution in Solution Explorer and select "Build Solution".

5. **Run the Project**:
   - Ensure the web project is set as the "Startup" and "Web" Projects.
   - Press `F5` or click on the "Start" button to run the project.
   - ![Startup setup example](assets/StartupProjects.png)

## Questions or Issues?
If you have any questions about the project or encounter any issues, please feel free to [post them here](https://github.com/ed-m-jp/Calendar/issues).


## Appendix

[Swagger Documentation](documents/swagger.json)

## Authors

* **Maire Edward**