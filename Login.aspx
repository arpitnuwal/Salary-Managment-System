<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Login" %>

<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Login &mdash; Welcome Back</title>
    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;500;600;700&display=swap" rel="stylesheet" />
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.0/css/all.min.css" rel="stylesheet" />

    <style>
        *, *::before, *::after { box-sizing: border-box; margin: 0; padding: 0; }

        :root {
            --bg-start:   #0f0c29;
            --bg-mid:     #302b63;
            --bg-end:     #24243e;
            --card-bg:    rgba(255,255,255,0.10);
            --card-border:rgba(255,255,255,0.22);
            --input-bg:   rgba(255,255,255,0.08);
            --input-focus:rgba(255,255,255,0.18);
            --btn-start:  #f7971e;
            --btn-end:    #e040fb;
            --text-main:  #ffffff;
            --text-muted: rgba(255,255,255,0.55);
            --text-label: rgba(255,255,255,0.80);
            --accent:     #e040fb;
            --error:      #ff6b8a;
            --radius:     18px;
            --transition: 0.3s cubic-bezier(.4,0,.2,1);
        }

        /* ─── Background ─────────────────────────────────── */
        body {
            min-height: 100vh;
            font-family: 'Poppins', 'Segoe UI', sans-serif;
            background: linear-gradient(135deg, var(--bg-start) 0%, var(--bg-mid) 50%, var(--bg-end) 100%);
            display: flex;
            align-items: center;
            justify-content: center;
            overflow: hidden;
            position: relative;
        }

        /* floating orbs */
        .orb {
            position: fixed;
            border-radius: 50%;
            filter: blur(80px);
            opacity: 0.45;
            animation: drift 14s ease-in-out infinite alternate;
            pointer-events: none;
        }
        .orb-1 { width: 500px; height: 500px; background: radial-gradient(circle, #7b2ff7, transparent 70%); top: -120px; left: -120px; animation-delay: 0s; }
        .orb-2 { width: 400px; height: 400px; background: radial-gradient(circle, #e040fb, transparent 70%); bottom: -100px; right: -80px;  animation-delay: -5s; }
        .orb-3 { width: 300px; height: 300px; background: radial-gradient(circle, #00b4d8, transparent 70%); top: 40%;  left: 55%;  animation-delay: -9s; }

        @keyframes drift {
            from { transform: translate(0,0) scale(1); }
            to   { transform: translate(40px, 30px) scale(1.08); }
        }

        /* floating geometric shapes */
        .shape {
            position: fixed;
            border: 1.5px solid rgba(255,255,255,0.10);
            border-radius: 50%;
            animation: spin linear infinite;
            pointer-events: none;
        }
        .shape-1 { width:120px; height:120px; top:8%;  left:6%;  animation-duration:22s; }
        .shape-2 { width: 70px; height: 70px;  bottom:12%; left:10%; animation-duration:18s; animation-direction:reverse; }
        .shape-3 { width: 90px; height: 90px;  top:60%;    right:5%; animation-duration:26s; border-radius:30% 70% 70% 30% / 30% 30% 70% 70%; }

        @keyframes spin {
            from { transform: rotate(0deg); }
            to   { transform: rotate(360deg); }
        }

        /* ─── Card ──────────────────────────────────────── */
        .login-card {
            position: relative;
            z-index: 10;
            width: 100%;
            max-width: 420px;
            margin: 24px;
            background: var(--card-bg);
            backdrop-filter: blur(24px) saturate(180%);
            -webkit-backdrop-filter: blur(24px) saturate(180%);
            border: 1px solid var(--card-border);
            border-radius: 28px;
            padding: 44px 40px 38px;
            box-shadow:
                0 8px 32px rgba(0,0,0,0.45),
                0 1px 0 rgba(255,255,255,0.12) inset,
                0 -1px 0 rgba(0,0,0,0.25) inset;
            animation: cardIn 0.7s cubic-bezier(.22,1,.36,1) both;
        }

        @keyframes cardIn {
            from { opacity:0; transform: translateY(36px) scale(0.97); }
            to   { opacity:1; transform: translateY(0)    scale(1);    }
        }

        /* top accent bar */
        .login-card::before {
            content: '';
            position: absolute;
            top: 0; left: 10%; right: 10%;
            height: 3px;
            background: linear-gradient(90deg, var(--btn-start), var(--btn-end));
            border-radius: 0 0 4px 4px;
            opacity: 0.9;
        }

        /* ─── Logo ───────────────────────────────────────── */
        .logo-wrap {
            display: flex;
            justify-content: center;
            margin-bottom: 22px;
        }
        .logo-icon {
            width: 60px; height: 60px;
            background: linear-gradient(135deg, #7b2ff7, #e040fb);
            border-radius: 16px;
            display: flex; align-items: center; justify-content: center;
            box-shadow: 0 8px 24px rgba(123,47,247,0.45);
            font-size: 26px;
            color: #fff;
            position: relative;
            overflow: hidden;
        }
        .logo-icon::after {
            content: '';
            position: absolute;
            inset: 0;
            background: linear-gradient(135deg, rgba(255,255,255,0.25), transparent);
            border-radius: inherit;
        }

        /* ─── Headings ───────────────────────────────────── */
        .heading { text-align: center; margin-bottom: 28px; }
        .heading h1 {
            font-size: 1.75rem;
            font-weight: 700;
            color: var(--text-main);
            letter-spacing: -0.5px;
            line-height: 1.2;
        }
        .heading p {
            margin-top: 6px;
            font-size: 0.875rem;
            color: var(--text-muted);
            font-weight: 300;
        }

        /* ─── Error panel ────────────────────────────────── */
        .error-panel {
            background: rgba(255, 107, 138, 0.15);
            border: 1px solid rgba(255,107,138,0.35);
            border-radius: 10px;
            padding: 10px 14px;
            margin-bottom: 18px;
            color: var(--error);
            font-size: 0.82rem;
            font-weight: 500;
            display: flex;
            align-items: center;
            gap: 8px;
            animation: fadeIn 0.3s ease both;
        }
        @keyframes fadeIn { from{opacity:0;transform:translateY(-6px)} to{opacity:1;transform:translateY(0)} }

        /* ─── Form fields ────────────────────────────────── */
        .field-group { margin-bottom: 16px; }

        .field-label {
            display: block;
            font-size: 0.78rem;
            font-weight: 500;
            color: var(--text-label);
            margin-bottom: 7px;
            letter-spacing: 0.3px;
        }

        .input-wrap {
            position: relative;
            display: flex;
            align-items: center;
        }
        .input-icon {
            position: absolute;
            left: 14px;
            color: var(--text-muted);
            font-size: 0.95rem;
            pointer-events: none;
            transition: color var(--transition);
        }
        .input-wrap:focus-within .input-icon { color: var(--accent); }

        .input-wrap input[type="text"],
        .input-wrap input[type="password"] {
            width: 100%;
            background: var(--input-bg);
            border: 1px solid rgba(255,255,255,0.14);
            border-radius: 12px;
            padding: 13px 14px 13px 40px;
            font-family: inherit;
            font-size: 0.9rem;
            font-weight: 400;
            color: var(--text-main);
            outline: none;
            transition: background var(--transition), border-color var(--transition), box-shadow var(--transition);
            -webkit-autofill-selected: none;
        }
        .input-wrap input:focus {
            background: var(--input-focus);
            border-color: rgba(224,64,251,0.6);
            box-shadow: 0 0 0 3px rgba(224,64,251,0.15);
        }
        .input-wrap input::placeholder { color: rgba(255,255,255,0.30); }

        /* password toggle */
        .toggle-pw {
            position: absolute;
            right: 13px;
            background: none;
            border: none;
            color: var(--text-muted);
            cursor: pointer;
            font-size: 0.9rem;
            padding: 4px;
            transition: color var(--transition);
        }
        .toggle-pw:hover { color: var(--accent); }

        /* ─── Remember / Forgot ──────────────────────────── */
        .row-meta {
            display: flex;
            align-items: center;
            justify-content: space-between;
            margin: 4px 0 22px;
        }
        .remember-label {
            display: flex;
            align-items: center;
            gap: 8px;
            cursor: pointer;
            font-size: 0.82rem;
            color: var(--text-muted);
            user-select: none;
        }
        .remember-label input[type="checkbox"] {
            appearance: none;
            width: 16px; height: 16px;
            border: 1.5px solid rgba(255,255,255,0.30);
            border-radius: 5px;
            background: var(--input-bg);
            cursor: pointer;
            position: relative;
            flex-shrink: 0;
            transition: background var(--transition), border-color var(--transition);
        }
        .remember-label input[type="checkbox"]:checked {
            background: linear-gradient(135deg, #7b2ff7, #e040fb);
            border-color: #e040fb;
        }
        .remember-label input[type="checkbox"]:checked::after {
            content: '';
            position: absolute;
            top: 2px; left: 4.5px;
            width: 5px; height: 8px;
            border: 2px solid #fff;
            border-top: none; border-left: none;
            transform: rotate(45deg);
        }
        .forgot-link {
            font-size: 0.82rem;
            color: var(--accent);
            text-decoration: none;
            font-weight: 500;
            transition: opacity var(--transition);
        }
        .forgot-link:hover { opacity: 0.75; text-decoration: underline; }

        /* ─── Login button ───────────────────────────────── */
        .btn-login {
            width: 100%;
            padding: 14px;
            background: linear-gradient(135deg, var(--btn-start) 0%, #f953c6 50%, var(--btn-end) 100%);
            background-size: 200% 200%;
            border: none;
            border-radius: 14px;
            color: #fff;
            font-family: inherit;
            font-size: 0.95rem;
            font-weight: 600;
            letter-spacing: 0.5px;
            cursor: pointer;
            transition: background-position var(--transition), transform var(--transition), box-shadow var(--transition);
            box-shadow: 0 6px 24px rgba(249,83,198,0.40);
            position: relative;
            overflow: hidden;
        }
        .btn-login::before {
            content: '';
            position: absolute;
            inset: 0;
            background: rgba(255,255,255,0);
            transition: background var(--transition);
        }
        .btn-login:hover {
            background-position: right center;
            transform: translateY(-2px);
            box-shadow: 0 10px 32px rgba(249,83,198,0.55);
        }
        .btn-login:hover::before { background: rgba(255,255,255,0.07); }
        .btn-login:active { transform: translateY(0); }

        /* ─── Divider ────────────────────────────────────── */
        .divider {
            display: flex;
            align-items: center;
            gap: 12px;
            margin: 22px 0;
        }
        .divider span { flex: 1; height: 1px; background: rgba(255,255,255,0.12); }
        .divider p { font-size: 0.75rem; color: var(--text-muted); white-space: nowrap; }

        /* ─── Social buttons ─────────────────────────────── */
        .social-row {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 12px;
        }
        .btn-social {
            display: flex;
            align-items: center;
            justify-content: center;
            gap: 8px;
            padding: 11px 10px;
            background: var(--input-bg);
            border: 1px solid rgba(255,255,255,0.14);
            border-radius: 12px;
            color: var(--text-main);
            font-family: inherit;
            font-size: 0.82rem;
            font-weight: 500;
            cursor: pointer;
            transition: background var(--transition), border-color var(--transition), transform var(--transition);
            text-decoration: none;
        }
        .btn-social:hover {
            background: var(--input-focus);
            border-color: rgba(255,255,255,0.28);
            transform: translateY(-1px);
        }
        .btn-social.google i  { color: #ea4335; }
        .btn-social.facebook i { color: #1877f2; }

        /* ─── Sign-up nudge ──────────────────────────────── */
        .signup-row {
            text-align: center;
            margin-top: 22px;
            font-size: 0.82rem;
            color: var(--text-muted);
        }
        .signup-row a {
            color: var(--accent);
            font-weight: 600;
            text-decoration: none;
            transition: opacity var(--transition);
        }
        .signup-row a:hover { opacity: 0.75; text-decoration: underline; }

        /* ─── Responsive ─────────────────────────────────── */
        @media (max-width: 480px) {
            .login-card { padding: 34px 24px 28px; }
            .heading h1 { font-size: 1.5rem; }
        }
    </style>
</head>
<body>
    <!-- Background atmosphere -->
    <div class="orb orb-1"></div>
    <div class="orb orb-2"></div>
    <div class="orb orb-3"></div>
    <div class="shape shape-1"></div>
    <div class="shape shape-2"></div>
    <div class="shape shape-3"></div>

    <form id="frmLogin" runat="server" autocomplete="off">
        <div class="login-card">

            <!-- Logo -->
            <div class="logo-wrap">
                <div class="logo-icon">
                    <i class="fa-solid fa-bolt-lightning"></i>
                </div>
            </div>

            <!-- Headings -->
            <div class="heading">
                <h1>Welcome Back</h1>
                <p>Login to your account</p>
            </div>

            <!-- Server-side error message (hidden by default) -->
            <asp:Panel ID="pnlError" runat="server" Visible="false" CssClass="error-panel">
                <i class="fa-solid fa-circle-exclamation"></i>
                <asp:Label ID="lblErrorMsg" runat="server" Text="Invalid username or password." />
            </asp:Panel>

            <!-- Username -->
            <div class="field-group">
                <label class="field-label" for="txtUsername">Username</label>
                <div class="input-wrap">
                    <i class="fa-solid fa-user input-icon"></i>
                    <asp:TextBox ID="txtUsername"
                                 runat="server"
                                 CssClass=""
                                 placeholder="Enter your username"
                                 MaxLength="100"
                                 ClientIDMode="Static" />
                </div>
                <asp:RequiredFieldValidator ID="rfvUsername" runat="server"
                    ControlToValidate="txtUsername"
                    ErrorMessage="Username is required."
                    CssClass="error-panel"
                    Display="Dynamic"
                    Style="margin-top:6px;" />
            </div>

            <!-- Password -->
            <div class="field-group">
                <label class="field-label" for="txtPassword">Password</label>
                <div class="input-wrap">
                    <i class="fa-solid fa-lock input-icon"></i>
                    <asp:TextBox ID="txtPassword"
                                 runat="server"
                                 TextMode="Password"
                                 placeholder="Enter your password"
                                 MaxLength="200"
                                 ClientIDMode="Static" />
                    <button type="button" class="toggle-pw" onclick="togglePassword()" title="Show/hide password">
                        <i id="eyeIcon" class="fa-regular fa-eye"></i>
                    </button>
                </div>
                <asp:RequiredFieldValidator ID="rfvPassword" runat="server"
                    ControlToValidate="txtPassword"
                    ErrorMessage="Password is required."
                    CssClass="error-panel"
                    Display="Dynamic"
                    Style="margin-top:6px;" />
            </div>

            <!-- Remember Me + Forgot Password -->
            <div class="row-meta">
                <label class="remember-label">
                    <asp:CheckBox ID="chkRemember" runat="server" ClientIDMode="Static" />
                    Remember me
                </label>
              
            </div>

            <!-- Login Button -->
            <asp:Button ID="btnLogin"
                        runat="server"
                        Text="Sign In"
                        CssClass="btn-login"
                        OnClick="btnLogin_Click"
                        ClientIDMode="Static" />

            <!-- Divider -->
          

            <!-- Social Buttons -->
          

            <!-- Sign-up -->
          
        </div>
    </form>

    <script>
        // Password visibility toggle
        function togglePassword() {
            var pw = document.getElementById('txtPassword');
            var icon = document.getElementById('eyeIcon');
            if (pw.type === 'password') {
                pw.type = 'text';
                icon.classList.replace('fa-eye', 'fa-eye-slash');
            } else {
                pw.type = 'password';
                icon.classList.replace('fa-eye-slash', 'fa-eye');
            }
        }

        // Card entrance stagger
        document.querySelectorAll('.field-group, .row-meta, .btn-login, .divider, .social-row, .signup-row')
            .forEach(function (el, i) {
                el.style.opacity = '0';
                el.style.transform = 'translateY(12px)';
                el.style.transition = 'opacity 0.45s ease ' + (0.18 + i * 0.07) + 's, transform 0.45s ease ' + (0.18 + i * 0.07) + 's';
                requestAnimationFrame(function () {
                    requestAnimationFrame(function () {
                        el.style.opacity = '1';
                        el.style.transform = 'translateY(0)';
                    });
                });
            });
    </script>
</body>
</html>
