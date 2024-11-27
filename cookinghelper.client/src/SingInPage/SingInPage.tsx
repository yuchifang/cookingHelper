import Box from "@mui/material/Box";
import Button from "@mui/material/Button";
import Checkbox from "@mui/material/Checkbox";
import FormControlLabel from "@mui/material/FormControlLabel";
import FormLabel from "@mui/material/FormLabel";
import FormControl from "@mui/material/FormControl";
import Link from "@mui/material/Link";
import TextField from "@mui/material/TextField";
import Typography from "@mui/material/Typography";
import Stack from "@mui/material/Stack";
import MuiCard from "@mui/material/Card";
import { styled } from "@mui/system";
import ForgotPassword from "../ForgotPassword";
import { redirect, useNavigate } from "react-router-dom";
import { useRef, useState } from "react";
import Alert from "@mui/material/Alert";
import { AxiosError } from "axios";
import CircularProgress from "@mui/material/CircularProgress";
import axiosInterceptor from "../axiosInterceptor";

//! Aspnet Core Identity ? search?

//!  Azure Table Storage.

//! Microsoft identity platform

//! 全部都用 axiosInterceptor, 全部都要加 錯誤處理, err.message
// axiosInterceptor 在特定 api 加 retry

export async function loader() {
  const response = await axiosInterceptor.get("/api/AccountIdentity/status");
  if (response?.data?.isAuthenticated) {
    return redirect("/admin"); // 已登入，跳轉到首頁
  }
  return null;
}

export function SignInPage() {
  const [emailError, setEmailError] = useState(false);
  const [emailErrorMessage, setEmailErrorMessage] = useState("");
  const [passwordError, setPasswordError] = useState(false);
  const [passwordErrorMessage, setPasswordErrorMessage] = useState("");
  const [open, setOpen] = useState(false);
  const emailRef = useRef<HTMLInputElement>(null);
  const passwordRef = useRef<HTMLInputElement>(null);

  const [errorMessage, setErrorMessage] = useState<string>("");
  const navigate = useNavigate();
  const [isChecked, setIsChecked] = useState(false);
  const [loading, setLoading] = useState(false);

  const handleChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setIsChecked(event.target.checked);
  };

  const validateInputs = () => {
    let isValid = true;

    if (
      !emailRef.current ||
      !emailRef.current.value ||
      !/\S+@\S+\.\S+/.test(emailRef.current.value)
    ) {
      setEmailError(true);
      setEmailErrorMessage("Please enter a valid email address.");
      isValid = false;
    } else {
      setEmailError(false);
      setEmailErrorMessage("");
    }

    if (
      !passwordRef.current ||
      !passwordRef.current.value ||
      passwordRef.current.value.length < 6
    ) {
      setPasswordError(true);
      setPasswordErrorMessage("Password must be at least 6 characters long.");
      isValid = false;
    } else {
      setPasswordError(false);
      setPasswordErrorMessage("");
    }

    return isValid;
  };

  const handleSubmit = async () => {
    if (validateInputs()) {
      setLoading(true);
      try {
        const response = await axiosInterceptor.post(
          "api/AccountIdentity/login",
          {
            email: (emailRef.current as HTMLInputElement).value.trim(),
            password: (passwordRef.current as HTMLInputElement).value.trim(),
            rememberMe: isChecked,
          },
          {
            headers: {
              "Content-Type": "application/json",
            },
          },
        );
        setLoading(false);
        if (response.statusText === "OK") {
          return navigate("/admin", { replace: true });
        }
      } catch (error) {
        setLoading(false);

        const err = error as AxiosError<Error>;
        if (err.response) {
          setErrorMessage(err.response.data.message);
          return;
        }
        err.message && setErrorMessage(err.message);
      }
    }
  };

  const handleClickOpen = () => {
    setOpen(true);
  };

  const handleClose = () => {
    setOpen(false);
  };

  return (
    <>
      <SignInContainer direction="column" justifyContent="space-between">
        <Card variant="outlined">
          <Typography
            component="h1"
            variant="h4"
            sx={{ width: "100%", fontSize: "clamp(2rem, 10vw, 2.15rem)" }}
          >
            Sign in
          </Typography>

          <Box
            sx={{
              display: "flex",
              flexDirection: "column",
              width: "100%",
              gap: 2,
            }}
          >
            <FormControl>
              <Box sx={{ display: "flex", justifyContent: "start" }}>
                <FormLabel htmlFor="email">Email</FormLabel>
              </Box>
              <TextField
                inputRef={emailRef}
                error={emailError}
                helperText={emailErrorMessage}
                id="email"
                type="email"
                name="email"
                placeholder="your@email.com"
                autoComplete="email"
                autoFocus
                required
                fullWidth
                variant="outlined"
                color={emailError ? "error" : "primary"}
                sx={{ ariaLabel: "email" }}
              />
            </FormControl>
            <FormControl>
              <Box sx={{ display: "flex", justifyContent: "space-between" }}>
                <FormLabel htmlFor="password">Password</FormLabel>
                <Link
                  component="button"
                  type="button"
                  onClick={handleClickOpen}
                  variant="body2"
                  sx={{ alignSelf: "baseline" }}
                >
                  Forgot your password?
                </Link>
              </Box>
              <TextField
                inputRef={passwordRef}
                error={passwordError}
                helperText={passwordErrorMessage}
                name="password"
                placeholder="••••••"
                type="password"
                id="password"
                autoComplete="current-password"
                autoFocus
                required
                fullWidth
                variant="outlined"
                color={passwordError ? "error" : "primary"}
              />
            </FormControl>
            <FormControlLabel
              control={
                <Checkbox
                  checked={isChecked}
                  onChange={handleChange}
                  name="remember"
                  color="primary"
                />
              }
              label="Remember me"
            />
            {errorMessage != "" && (
              <Alert severity="error">{errorMessage}</Alert>
            )}
            <ForgotPassword open={open} handleClose={handleClose} />
            <Button
              type="submit"
              fullWidth
              variant="outlined"
              onClick={handleSubmit}
            >
              {loading ? <CircularProgress size="24.5px" /> : "Sign in"}
            </Button>
          </Box>
        </Card>
      </SignInContainer>
    </>
  );
}

const Card = styled(MuiCard)(({ theme }) => ({
  display: "flex",
  flexDirection: "column",
  alignSelf: "center",
  width: "100%",
  padding: theme.spacing(4),
  gap: theme.spacing(2),
  margin: "auto",
  [theme.breakpoints.up("sm")]: {
    maxWidth: "450px",
  },
  boxShadow:
    "hsla(220, 30%, 5%, 0.05) 0px 5px 15px 0px, hsla(220, 25%, 10%, 0.05) 0px 15px 35px -5px",
  ...theme.applyStyles("dark", {
    boxShadow:
      "hsla(220, 30%, 5%, 0.5) 0px 5px 15px 0px, hsla(220, 25%, 10%, 0.08) 0px 15px 35px -5px",
  }),
}));

const SignInContainer = styled(Stack)(({ theme }) => ({
  minHeight: "100%",
  minWidth: "500px",
  padding: theme.spacing(2),
  [theme.breakpoints.up("sm")]: {
    padding: theme.spacing(4),
  },
  "&::before": {
    content: '""',
    display: "block",
    position: "absolute",
    zIndex: -1,
    inset: 0,
    backgroundImage:
      "radial-gradient(ellipse at 50% 50%, hsl(210, 100%, 97%), hsl(0, 0%, 100%))",
    backgroundRepeat: "no-repeat",
    ...theme.applyStyles("dark", {
      backgroundImage:
        "radial-gradient(at 50% 50%, hsla(210, 100%, 16%, 0.5), hsl(220, 30%, 5%))",
    }),
  },
}));
