<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LOGIN.aspx.cs" Inherits="VMS_1.LOGIN" %> 
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Login</title> 
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
            display: flex;
            justify-content: center;
            align-items: center;
        }

        .container {
            background-color: #fff;
            border-radius: 8px;
            box-shadow: 0 0 20px rgba(0, 0, 0, 0.1);
            max-width: 400px;
            width: 100%;
            padding: 40px;
            box-sizing: border-box;
            text-align: center;
        }

        .title {
            color: var(--primary-color);
            font-size: 32px;
            font-weight: bold;
            margin-bottom: 20px;
        }

        .form-group {
            margin-bottom: 20px;
            text-align: left;
        }

        .form-group label {
            color: #555555;
            font-weight: 500;
            display: block;
            margin-bottom: 5px;
        }

        .form-group input {
            width: calc(100% - 24px);
            padding: 14px;
            margin-bottom: 10px;
            border: 1px solid #cccccc;
            border-radius: 6px;
            box-sizing: border-box;
            font-size: 16px;
            transition: border-color 0.3s;
            outline: none;
        }

        .form-group input:focus {
            border-color: var(--primary-color);
        }

        .alert {
            color: red;
            font-size: 12px;
            margin-top: 5px;
            text-align: left;
        }

        .btn-login {
            background-color: var(--primary-color);
            color: #fff;
            padding: 14px 0;
            border: none;
            border-radius: 6px;
            cursor: pointer;
            font-size: 16px;
            transition: background-color 0.3s;
            width: calc(100% - 24px);
            max-width: 300px;
            margin: 0 auto;
        }

        .btn-login:hover {
            background-color: var(--hover-color);
        }

        .forgot-password {
            color: var(--primary-color);
            text-decoration: none;
            font-size: 14px;
            margin-top: 10px;
            display: inline-block;
        }

        .forgot-password:hover {
            text-decoration: underline;
        }

        .register-link {
            color: var(--primary-color);
            text-decoration: none;
            display: block;
            margin-top: 20px;
            font-weight: bold;
        }

        .register-link:hover {
            text-decoration: underline;
        }
    </style>
</head>
<body> 
    <form id="form1" runat="server" class="container">
        <div class="title">
            VICTUALLING MANAGEMENT SYSTEM
        </div>
        <div class="form-group">
            <label for="UserName">NUD ID:</label>
            <asp:TextBox ID="UserName" runat="server" placeholder="NUD ID"></asp:TextBox>
            <div id="usernameAlert" class="alert"></div>
        </div>
        <div class="form-group">
            <label for="Password">Password:</label>
            <asp:TextBox ID="Password" runat="server" TextMode="Password" placeholder="Password"></asp:TextBox>
            <div id="passwordAlert" class="alert"></div>
        </div>
        <asp:Button ID="btnLogin" runat="server" Text="Login" CssClass="btn-login" OnClick="btnLogin_Click" OnClientClick="return validateLogin();" />
        <a href="ForgetPassword.aspx" class="forgot-password">Forgot Password?</a>
        <br />
        <asp:Label ID="ErrorLabel" runat="server" Visible="False" CssClass="alert-danger"></asp:Label>
        <a href="UserRegister.aspx" class="register-link">Don't have an account? Register here.</a>
    </form>

    <script>
        function validateLogin() {
            var username = document.getElementById('<%= UserName.ClientID %>').value.trim();
            var password = document.getElementById('<%= Password.ClientID %>').value.trim();
            var usernameAlert = document.getElementById('usernameAlert');
            var passwordAlert = document.getElementById('passwordAlert');

            if (username === '') {
                usernameAlert.innerHTML = 'Please fill out the NUD ID field.';
                usernameAlert.style.display = 'block';
                return false; // Prevent form submission
            } else {
                usernameAlert.style.display = 'none';
            }

            if (password === '') {
                passwordAlert.innerHTML = 'Please fill out the Password field.';
                passwordAlert.style.display = 'block';
                return false; // Prevent form submission
            } else {
                passwordAlert.style.display = 'none';
            }

            return true; // Allow form submission
        }
    </script>
</body>
</html>
