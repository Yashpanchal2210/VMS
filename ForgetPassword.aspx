<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ForgetPassword.aspx.cs" Inherits="VMS_1.ForgetPassword" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Forget Password</title>
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
            background-color: var(--primary-color);
            transition: background-color 0.3s;
            position: relative;
        }

        .container {
            display: flex;
            justify-content: center;
            align-items: center;
            padding: 10px;
        }

        .title {
            color: #ffffff;
            font-size: 32px;
            font-weight: bold;
        }

        .theme-selector {
            position: absolute;
            top: 10px;
            right: 10px;
            display: flex;
            align-items: center;
        }

        .theme-selector label {
            color: #ffffff;
            margin-right: 5px;
        }

        .theme-selector select {
            padding: 5px;
            border-radius: 4px;
        }

        form {
            background-color: #ffffff;
            padding: 40px;
            border-radius: 8px;
            box-shadow: 0 0 20px rgba(0, 0, 0, 0.1);
            max-width: 400px;
            width: 100%;
            text-align: center;
            transition: transform 0.3s ease;
            margin: 20px auto;
        }

        h2 {
            margin-bottom: 20px;
            color: #333333;
            font-style: var(--font-style);
            font-weight: var(--font-weight);
        }

        label {
            display: block;
            margin-bottom: 10px;
            color: #555555;
            font-weight: 500;
        }

        input[type="text"],
        input[type="password"],
        select {
            width: calc(100% - 24px);
            padding: 14px;
            margin-bottom: 20px;
            border: 1px solid #cccccc;
            border-radius: 6px;
            box-sizing: border-box;
            font-size: 16px;
            transition: border-color 0.3s;
        }

        input[type="button"] {
            background-color: var(--primary-color);
            color: #ffffff;
            padding: 14px 0;
            border: none;
            border-radius: 6px;
            cursor: pointer;
            font-size: 16px;
            transition: background-color 0.3s;
            width: 100%;
            max-width: 300px;
            margin: 0 auto;
        }

        input[type="button"]:hover {
            background-color: var(--hover-color);
        }

        .error {
            color: red;
            margin-top: 10px;
            font-size: 14px;
        }

        .error:focus {
            border-color: red;
        }
    </style>
</head>
<body>
    <div class="container">
        <div class="title">
            VICTUALLING MANAGEMENT SYSTEM
        </div>
        <div class="theme-selector">
            <label for="ddlTheme">Choose Theme:</label>
            <select id="ddlTheme">
                <option value="blue">Blue</option>
                <option value="dark">Dark</option>
            </select>
        </div>
    </div>
    <form id="formForgetPassword" runat="server">
        <h2>Forget Password</h2>
        <div>
            <label for="txtNUDID">NUD ID:</label>
            <asp:TextBox ID="txtNUDID" runat="server" placeholder="Enter your NUD ID" required></asp:TextBox>
        </div>
        <div>
            <label for="ddlSecretQuestion">Select Secret Question:</label>
            <asp:DropDownList ID="ddlSecretQuestion" runat="server" required>
                <asp:ListItem Text="-- Select Secret Question --" Value="0"></asp:ListItem>
                <asp:ListItem Text="Name of your First School" Value="1"></asp:ListItem>
                <asp:ListItem Text="Mother's Maiden Name" Value="2"></asp:ListItem>
                <asp:ListItem Text="Unit of first appointment" Value="3"></asp:ListItem>
                <asp:ListItem Text="Name of favourite book" Value="4"></asp:ListItem>
            </asp:DropDownList>
        </div>
        <div>
            <label for="txtAnswer">Answer:</label>
            <asp:TextBox ID="txtAnswer" runat="server" placeholder="Enter your answer" required></asp:TextBox>
        </div>
        <div>
            &nbsp;&nbsp;
            <asp:Button ID="btnSubmit" runat="server" Text="Submit" OnClick="btnSubmit_Click" />
            <br />
            <asp:Label ID="ErrorLabel" runat="server" Visible="False" CssClass="error"></asp:Label>
            <a href="LOGIN.aspx" class="register-link">Back to Login Page</a>
        </div>
    </form>

    <script>
        document.getElementById("ddlTheme").addEventListener("change", function () {
            var selectedTheme = this.value;
            setTheme(selectedTheme);
        });

        // Initial theme setup
        setTheme("blue");

        function setTheme(theme) {
            switch (theme) {
                case 'dark':
                    document.documentElement.style.setProperty('--primary-color', '#2c3e50');
                    document.documentElement.style.setProperty('--hover-color', lightenDarkenColor('#2c3e50', 20));
                    break;
                case 'blue':
                default: // Default to blue if no specific theme is provided
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
