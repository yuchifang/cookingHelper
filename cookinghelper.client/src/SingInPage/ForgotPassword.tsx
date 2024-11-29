import * as React from "react";
import Button from "@mui/material/Button";
import Dialog from "@mui/material/Dialog";
import DialogActions from "@mui/material/DialogActions";
import DialogContent from "@mui/material/DialogContent";
import DialogContentText from "@mui/material/DialogContentText";
import DialogTitle from "@mui/material/DialogTitle";
import OutlinedInput from "@mui/material/OutlinedInput";
import axiosInterceptor from "../axiosInterceptor";
import { useRef } from "react";
import { AxiosError } from "axios";

interface ForgotPasswordProps {
  open: boolean;
  handleClose: () => void;
  handlePrompt: React.Dispatch<React.SetStateAction<string>>;
}

export default function ForgotPassword({
  open,
  handleClose,
  handlePrompt,
}: ForgotPasswordProps) {
  const emailRef = useRef<HTMLInputElement>(null);

  const SendEmail = async () => {
    try {
      const response = await axiosInterceptor.post(
        "api/AccountIdentity/forgot-password",
        {
          email: (emailRef.current as HTMLInputElement).value.trim(),
        },
        {
          headers: {
            "Content-Type": "application/json",
          },
        },
      );
      if (response.statusText === "OK") {
        console.log(response);
        response.data && handlePrompt(response.data.message);
      }
    } catch (error) {
      const err = error as AxiosError<Error>;
      err.message && handlePrompt(err.message);
    }
  };
  return (
    <Dialog
      open={open}
      onClose={handleClose}
      PaperProps={{
        component: "form",
        onSubmit: async (event: React.FormEvent<HTMLFormElement>) => {
          event.preventDefault();

          await SendEmail();

          handleClose();
        },
      }}
    >
      <DialogTitle>Reset password</DialogTitle>
      <DialogContent sx={{ display: "flex", flexDirection: "column", gap: 2 }}>
        <DialogContentText>
          Enter your account&apos;s email address, and we&apos;ll send you a
          link to reset your password.
        </DialogContentText>
        <OutlinedInput
          inputRef={emailRef}
          autoFocus
          required
          margin="dense"
          id="email"
          name="email"
          placeholder="Email address"
          type="email"
          fullWidth
        />
      </DialogContent>
      <DialogActions sx={{ pb: 3, px: 3 }}>
        <Button onClick={handleClose}>Cancel</Button>
        <Button variant="contained" type="submit">
          Continue
        </Button>
      </DialogActions>
    </Dialog>
  );
}
