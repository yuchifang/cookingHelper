import Button from "@mui/material/Button";

import FormControl from "@mui/material/FormControl";

import FormLabel from "@mui/material/FormLabel";

import TextField from "@mui/material/TextField";
import Typography from "@mui/material/Typography";
import Box from "@mui/system/Box";
import { useRef, useState } from "react";
import MuiCard from "@mui/material/Card";
import { styled } from "@mui/system";
import Stack from "@mui/material/Stack";

import Select from "@mui/material/Select";
import MenuItem from "@mui/material/MenuItem";
import FormHelperText from "@mui/material/FormHelperText";

import { AxiosError } from "axios";
import CircularProgress from "@mui/material/CircularProgress";
import Alert from "@mui/material/Alert";
import Snackbar from "@mui/material/Snackbar";
import axiosInstance from "../axiosInterceptor";

interface ApiStatus {
  status: string;
  message: string;
}

export default function AccountBlock() {
  const [emailError, setEmailError] = useState(false);
  const [emailErrorMessage, setEmailErrorMessage] = useState("");
  const [passwordError, setPasswordError] = useState(false);
  const [passwordErrorMessage, setPasswordErrorMessage] = useState("");
  const [apiStatus, setApiStatus] = useState<ApiStatus>({
    status: "init",
    message: "",
  });

  const passwordRef = useRef<HTMLInputElement>(null);
  const emailRef = useRef<HTMLInputElement>(null);
  const permissionRef = useRef<HTMLSelectElement>(null);

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
    if (!validateInputs()) {
      return;
    }
    setApiStatus(() => ({ status: "loading", message: "" }));
    if (apiStatus.status === "loading") return;
    try {
      await axiosInstance.post(
        "/api/AccountIdentity/register",
        {
          email: emailRef.current!.value.trim(),
          password: passwordRef.current!.value.trim(),
          permission: permissionRef.current!.value,
        },
        {
          headers: {
            "Content-Type": "application/json",
          },
        },
      );

      setApiStatus(() => ({ status: "success", message: "" }));
    } catch (error) {
      const err = error as AxiosError<Error>;

      if (err.response) {
        setApiStatus(() => ({
          status: "error",
          message: err.response!.data.message,
        }));
        return;
      }

      err.message &&
        setApiStatus(() => ({
          status: "error",
          message: err.message,
        }));
    }
  };

  const handleClose = () => {
    setApiStatus(() => ({ status: "init", message: "" }));
  };

  return (
    <SignUpContainer>
      <Card>
        <Typography
          component="h1"
          variant="h4"
          sx={{ width: "100%", fontSize: "clamp(2rem, 10vw, 2.15rem)" }}
        >
          註冊帳號
        </Typography>

        <Box sx={{ display: "flex", flexDirection: "column", gap: 2 }}>
          <FormControl>
            <FormLabel htmlFor="email">Email</FormLabel>
            <TextField
              inputRef={emailRef}
              required
              fullWidth
              id="email"
              placeholder="your@email.com"
              name="email"
              autoComplete="email"
              variant="outlined"
              error={emailError}
              helperText={emailErrorMessage}
              color={passwordError ? "error" : "primary"}
            />
          </FormControl>
          <FormControl>
            <FormLabel htmlFor="password">Password</FormLabel>
            <TextField
              inputRef={passwordRef}
              required
              fullWidth
              name="password"
              placeholder="••••••"
              type="password"
              id="password"
              autoComplete="new-password"
              variant="outlined"
              error={passwordError}
              helperText={passwordErrorMessage}
              color={passwordError ? "error" : "primary"}
            />
          </FormControl>
          <FormControl fullWidth>
            <Select
              name="permission"
              defaultValue="guest"
              inputRef={permissionRef}
              displayEmpty
              inputProps={{ "aria-label": "Without label" }}
            >
              <MenuItem value={"guest"}>guest</MenuItem>
              <MenuItem value={"admin"}>admin</MenuItem>
            </Select>
            <FormHelperText>Permission</FormHelperText>
          </FormControl>
          <Button
            type="submit"
            fullWidth
            variant="outlined"
            onClick={handleSubmit}
          >
            {apiStatus.status === "loading" ? (
              <CircularProgress size="25px" />
            ) : (
              "register"
            )}
          </Button>
        </Box>
        {apiStatus.status === "error" && (
          <Alert severity="error">{apiStatus.message}</Alert>
        )}
      </Card>
      <Snackbar
        open={apiStatus.status === "success"}
        autoHideDuration={1500}
        message="註冊成功"
        onClose={handleClose}
      />
    </SignUpContainer>
  );
}

const Card = styled(MuiCard)(({ theme }) => ({
  display: "flex",
  flexDirection: "column",

  maxWidth: "50%",
  padding: theme.spacing(4),
  gap: theme.spacing(2),
  boxShadow: "none",
}));

const SignUpContainer = styled(Stack)(({ theme }) => ({
  minHeight: "507px",
  padding: theme.spacing(2),
  [theme.breakpoints.up("sm")]: {
    padding: theme.spacing(4),
  },
}));
