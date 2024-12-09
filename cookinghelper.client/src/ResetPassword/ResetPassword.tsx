import Stack from "@mui/material/Stack";
import { styled } from "@mui/system";
import MuiCard from "@mui/material/Card";
import { useNavigate, useSearchParams } from "react-router-dom";
import Typography from "@mui/material/Typography";
import Box from "@mui/material/Box";
import FormControl from "@mui/material/FormControl";

import TextField from "@mui/material/TextField";
import { useRef, useState } from "react";
import Alert from "@mui/material/Alert";
import CircularProgress from "@mui/material/CircularProgress";
import Button from "@mui/material/Button";
import FormLabel from "@mui/material/FormLabel";
import axiosInterceptor from "../axiosInterceptor";
import { AxiosError } from "axios";

interface MessageInfo {
  message: string;
  status: "error" | "success";
}
ResetPassword.displayName = "ResetPassword";
export function ResetPassword() {
  const resetPasswordRef = useRef<HTMLInputElement>(null);
  const confirmPasswordRef = useRef<HTMLInputElement>(null);
  const [searchParams] = useSearchParams();

  const [resetPasswordError, setResetPasswordError] = useState(false);
  const [resetPasswordErrorMessage, setResetPasswordErrorMessage] =
    useState("");

  const [confirmPasswordError, setConfirmPasswordError] = useState(false);
  const [confirmPasswordErrorMessage, setConfirmPasswordErrorMessage] =
    useState("");

  const [messageInfo, setMessageInfo] = useState<MessageInfo>({
    message: "",
    status: "success",
  });
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();

  if (
    searchParams.get("email") === null ||
    searchParams.get("token") === null
  ) {
    throw "error";
  }

  const validateInputs = () => {
    let isValid = true;

    if (
      !resetPasswordRef.current ||
      !resetPasswordRef.current.value ||
      resetPasswordRef.current.value.length < 6
    ) {
      setResetPasswordError(true);
      setResetPasswordErrorMessage(
        "Password must be at least 6 characters long.",
      );
      isValid = false;
    } else {
      setResetPasswordError(false);
      setResetPasswordErrorMessage("");
    }

    if (
      !resetPasswordRef.current ||
      !confirmPasswordRef.current ||
      !confirmPasswordRef.current.value ||
      confirmPasswordRef.current.value.length < 6 ||
      confirmPasswordRef.current.value !== resetPasswordRef.current.value
    ) {
      setConfirmPasswordError(true);
      setConfirmPasswordErrorMessage(
        "The reset password and confirmation password must be the same",
      );
      isValid = false;
    } else {
      setConfirmPasswordError(false);
      setConfirmPasswordErrorMessage("");
    }

    return isValid;
  };

  const handleSubmit = async () => {
    if (validateInputs()) {
      try {
        setLoading(true);
        await axiosInterceptor.post(
          "api/AccountIdentity/reset-password",
          {
            email: searchParams.get("email"),
            newPassword: (resetPasswordRef!.current as HTMLInputElement).value,
            token: searchParams.get("token"),
          },
          {
            headers: {
              "Content-Type": "application/json",
            },
          },
        );
        setLoading(false);
        navigate("/", { replace: true });
      } catch (error) {
        const err = error as AxiosError<Error>;

        if (err.response && err.response!.data.message) {
          setMessageInfo(() => ({
            status: "error",
            message: err.response!.data.message,
          }));
          return;
        }

        err.message &&
          setMessageInfo(() => ({ status: "error", message: err.message }));
      }
      setLoading(false);
    }
  };

  return (
    <ResetPasswordContainer direction="column" justifyContent="space-between">
      <Card variant="outlined">
        <Typography
          component="h1"
          variant="h4"
          sx={{ width: "100%", fontSize: "clamp(2rem, 10vw, 2.15rem)" }}
        >
          ResetPassword
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
              <FormLabel htmlFor="email">ResetPassword</FormLabel>
            </Box>
            <TextField
              inputRef={resetPasswordRef}
              error={resetPasswordError}
              helperText={resetPasswordErrorMessage}
              name="password"
              placeholder="••••••"
              type="password"
              id="password"
              autoComplete="current-password"
              autoFocus
              required
              fullWidth
              variant="outlined"
              color={resetPasswordError ? "error" : "primary"}
            />
          </FormControl>
          <FormControl>
            <Box sx={{ display: "flex", justifyContent: "start" }}>
              <FormLabel htmlFor="email">Confirm Password</FormLabel>
            </Box>
            <TextField
              inputRef={confirmPasswordRef}
              error={confirmPasswordError}
              helperText={confirmPasswordErrorMessage}
              name="confirmPassword"
              placeholder="••••••"
              type="password"
              id="password"
              autoComplete="current-password"
              autoFocus
              required
              fullWidth
              variant="outlined"
              color={resetPasswordError ? "error" : "primary"}
            />
          </FormControl>

          {messageInfo.message !== "" && (
            <Alert severity={messageInfo.status}>
              {messageInfo.message !== ""
                ? messageInfo.message
                : "Something went wrong"}
            </Alert>
          )}

          <Button
            type="submit"
            fullWidth
            variant="outlined"
            onClick={handleSubmit}
          >
            {loading ? <CircularProgress size="24.5px" /> : "Reset Password"}
          </Button>
        </Box>
      </Card>
    </ResetPasswordContainer>
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

const ResetPasswordContainer = styled(Stack)(({ theme }) => ({
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
