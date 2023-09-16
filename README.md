# NejTack
 An application that automatically sets your availability status to "unavailable" at the beginning of the day for the StudentConsulting staffing agency.

### Background
As of now, I'm employed part-time/extra at a logistics facility through a staffing agency. Due to the unpredictable nature of the job demand, the agency requires us to specify our weekly availability. Typically, work assignments are confirmed two to three days in advance. However, there are occasions when last-minute bookings occur, particularly on the same day. This poses a challenge for me as I work night shifts and need to adjust my sleep schedule accordingly. After reaching out to the agency regarding this issue, their solution was simple: update my availability in their app. While this resolved the problem in theory, remembering to make myself unavailable each day before a shift is not going to happend. So, I decided to put a little automation magic into play!

### Features
- Sets the availability status to "unavailable" at the beginning of the day.
- Automaticlly accept the new inquiries that is specified in the appsettings.json.

### Configurations
Rename the appsettings.EXAMPLE.json to appsettings.json.
```yaml
{
  "username": "MAIL", # Email.
  "password": "PASSWORD", # Password.
  "secret": "SECRET", # Account's secret.
  "availability": true, # Run the availability service (Needs restart).
  "autoresponse": { # Setting up the auto-response service (Hot-reloaded).
    "status": true,  
    "cycle": 300000, # Cycle time between checks in milliseconds.
    "accept": [ # List of intervals to be automaticlly accepted.
      {
        "start": "2023-09-29T19:00:00.000Z",
        "end": "2023-09-30T04:00:00.000Z"
      },
      {
        "start": "2023-10-28T19:00:00.000Z",
        "end": "2023-10-29T04:00:00.000Z"
      },
      {
        "start": "2023-10-05T21:00:00+02:00",
        "end": "2023-10-06T06:00:00+02:00"
      }
    ]
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "System": "Warning"
    }
  }
}
```
### Building and Running the Application
To build the application you need to install the dotnet sdk version 7.0.0, clone the repository:
```
> git clone git@github.com:nordotnu/NejTack.git
```
OR 
```
> git clone https://github.com/nordotnu/NejTack.git
```
Then navigate to the repository folder and build for your system:
```
> dotnet publish -c release
```
Run the application:
```
> ./bin/Release/net7.0/publish/
> ./NejTack
```
## Disclaimer: Educational and Demonstration Purposes Only

This program is provided solely for educational purposes and as a demonstration of skills. It is not intended to be used as a tool for any practical, real-world application. The following disclaimer outlines important information regarding the use and limitations of this program:

  1. Non-Production Use: This program is not suitable for use in production environments, critical systems, or any scenario where reliability, security, or data integrity is of concern.

   1. Limited Functionality: The program may lack essential features, optimizations, and robust error handling commonly found in production-ready software.

   1. No Warranty: This program is provided "as is" without any warranty, expressed or implied. There is no guarantee of fitness for a particular purpose or compatibility with any specific hardware or software configuration.

   1. No Support: We do not offer any support, maintenance, or updates for this program. You are responsible for any modifications or improvements required for your educational purposes.

   1. Security Risks: Using this program in any real-world application may expose your system to security risks, vulnerabilities, or unintended consequences. We strongly discourage its use for any such purposes.

   1. Data Loss: The program may not handle data safely, and its use may result in data loss or corruption. Do not use it with valuable or irreplaceable data.

  Compliance: Ensure that your use of this program complies with all applicable laws, regulations, and ethical standards. Misuse or unauthorized use of this program is strictly prohibited.

  Responsibility: You are solely responsible for any actions you take with this program. We will not be liable for any damages, losses, or consequences arising from its use.

By using this program, you acknowledge and agree to the terms of this disclaimer. If you require a tool for practical or production purposes, we recommend seeking professionally developed and supported software solutions that meet your specific needs and requirements.

Please exercise caution and discretion when using this program and always prioritize safety, security, and legality in your actions.
