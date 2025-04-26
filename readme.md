# OwlTech CPU Scheduler

> This application creates two new algorithms the Shortest Remaining Time First (SRTF) and the Highest Response Ratio Next (HRRN) on top of the existing work done here by [Francis Nweke](https://github.com/FrancisNweke/CPU-Simulator-GUI). It adds more UI interfaces for assessing the performance metrics of all algorithms.

## Prerequisites

Before running this application, make sure you have the following installed:

- [.NET SDK 9.0](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)

Check your .NET installation by running:

```bash
dotnet --version
```

You should see a version number starting with `9.`.


## How to Run

**NOTE** Make sure to navigate to the `/CpuSchedulingWinForms` folder before running the below commands.

1. **Restore dependencies**

If the project uses external packages, restore them with:

```bash
dotnet restore
```

2. **Build the application**

```bash
dotnet build
```

3. **Run the application**

```bash
dotnet run
```

This will automatically build (if necessary) and start the application.

## Running with Visual Studio / VS Code

- **Visual Studio Code**:  
  Open the project folder and install the recommended C# extension.  
  You can run the project using the integrated terminal:

```bash
dotnet run
```

Or by using the "Run and Debug" button.

## Usage

```
Please install the font: "IDAutomationHC39M Free Version". You can find it in the project root folder.
```

## License
This project is licensed under the terms of the [MIT license](https://choosealicense.com/licenses/mit/).
