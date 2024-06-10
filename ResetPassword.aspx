<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ResetPassword.aspx.cs" Inherits="VMS_1.ResetPassword" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Reset Password</title>
    <style>
        :root {
            --primary-color: #3498db;
            --hover-color: #2980b9;
            --font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            --font-style: normal;
            --font-weight: normal;
        }

        body {
            font-family: var(--font-family);
            margin: 0;
            padding: 0;
            height: 100vh;
            background-color: #f2f2f2;
            transition: background-color 0.3s;
            position: relative;
        }

        .container {
            width: 100%;
            max-width: 400px;
            margin: 0 auto;
            padding: 20px;
            background-color: #ffffff;
            border-radius: 8px;
            box-shadow: 0 0 20px rgba(0, 0, 0, 0.1);
            margin-top: 50px;
        }

        h1 {
            text-align: center;
            margin-bottom: 20px;
            color: var(--primary-color);
            font-style: var(--font-style);
            font-weight: var(--font-weight);
        }

        label {
            font-weight: bold;
            color: #555555;
        }

        input[type="password"],
        select {
            width: 100%;
            padding: 10px;
            margin: 8px 0;
            border: 1px solid #ccc;
            border-radius: 4px;
            box-sizing: border-box;
            font-size: 16px;
            transition: border-color 0.3s;
        }

        input[type="button"],
        input[type="submit"] {
            background-color: var(--primary-color);
            color: white;
            padding: 14px 20px;
            margin: 8px 0;
            border: none;
            border-radius: 4px;
            cursor: pointer;
            width: 100%;
            transition: background-color 0.3s;
        }

        input[type="button"]:hover,
        input[type="submit"]:hover {
            background-color: var(--hover-color);
        }

        .error {
            color: red;
            margin-top: 10px;
            font-size: 14px;
            text-align: center;
        }

        .validation-message {
            color: red;
            font-size: 12px;
            margin-top: 5px;
        }

        .theme-selector {
            position: absolute;
            top: 10px;
            right: 10px;
            display: flex;
            align-items: center;
        }

        .theme-selector label {
            margin-right: 5px;
        }

        .theme-selector select {
            padding: 5px;
            border-radius: 4px;
        }
    </style>
</head>
<body>
    <h1>VICTUALLING MANAGEMENT SYSTEM</h1>
    <div class="container">
        <div class="theme-selector">
            <label for="ddlTheme">Choose Theme:</label>
            <select id="ddlTheme" onchange="setTheme(this.value)">
                <option value="blue">Blue</option>
                <option value="dark">Dark</option>
            </select>
        </div>
        <form id="form1" runat="server">
            <div>
                <label for="txtNewPassword">New Password:</label>
                <asp:TextBox ID="txtNewPassword" runat="server" TextMode="Password" placeholder="Enter your new password" required></asp:TextBox>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="txtNewPassword" ErrorMessage="Password should be minimum 8 characters with at least one letter, one numeral, and one special character" ValidationExpression="^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}$"></asp:RegularExpressionValidator>
                <asp:Label ID="lblNewPasswordError" runat="server" CssClass="validation-message" Text="" Visible="False"></asp:Label>
            </div>
            <div>
                <label for="txtReenterPassword">Re-enter Password:</label>
                <asp:TextBox ID="txtReenterPassword" runat="server" TextMode="Password" placeholder="Re-enter your new password" required></asp:TextBox>
                
            </div>
            <asp:Button ID="btnReset" runat="server" Text="Reset Password" OnClick="btnReset_Click" OnClientClick="return validatePassword()" />
            <asp:Label ID="ErrorLabel" runat="server" Visible="False" CssClass="error"></asp:Label>
        &nbsp;</form>
    </div>

    <script>
        // Function to validate password and re-enter password
        function validatePassword() {
            var newPassword = document.getElementById('<%= txtNewPassword.ClientID %>').value;
            var reenterPassword = document.getElementById('<%= txtReenterPassword.ClientID %>').value;
            if (newPassword !== reenterPassword) {
                alert("Passwords do not match");
                return false;
            }
            return true;
        }

        // Function to set theme
        function setTheme(theme) {
            switch (theme) {
                case 'dark':
                    document.documentElement.style.setProperty('--primary-color', '#2c3e50');
                    document.documentElement.style.setProperty('--hover-color', lightenDarkenColor('#2c3e50', 20));
                    break;
                case 'blue':
                default:
                    document.documentElement.style.setProperty('--primary-color', '#3498db');
                    document.documentElement.style.setProperty('--hover-color', lightenDarkenColor('#3498db', -20));
                    break;
            }
        }

        // Function to lighten or darken a color
        function lightenDarkenColor(col, amt) {
            var usePound = false;
            if (col[0] === "#") {
                col = col.slice(1);
                usePound = true;
            }
            var num = parseInt(col, 16);
            var r = (num >> 16) + amt;
            if (r > 255) r = 255;
            else if (r < 0) r = 0;
            var b = ((num >> 8) & 0x00FF) + amt;
            if (b > 255) b = 255;
            else if (b < 0) b = 0;
            var g = (num & 0x0000FF) + amt;
            if (g > 255) g = 255;
            else if (g < 0) g = 0;
            return (usePound ? "#" : "") + (g | (b << 8) | (r << 16)).toString(16);
        }
    </script>
</body>
</html>
