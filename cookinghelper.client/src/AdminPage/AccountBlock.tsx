import Button from "@mui/material/Button";

import FormControl from "@mui/material/FormControl";

import FormLabel from "@mui/material/FormLabel";

import TextField from "@mui/material/TextField";
import Typography from "@mui/material/Typography";
import Box from "@mui/system/Box";
import { useState } from "react";
import MuiCard from "@mui/material/Card";
import { styled } from "@mui/system";
import Stack from "@mui/material/Stack";

import Select, { SelectChangeEvent } from "@mui/material/Select";
import MenuItem from "@mui/material/MenuItem";
import FormHelperText from "@mui/material/FormHelperText";
import { Params, useFetcher } from "react-router-dom";
import axios from "axios";

export async function action({
  request,
}: {
  params: Params;
  request: Request;
}) {
  const formData: FormData = await request.formData();
  const response = await axios.post(
    "/api/AccountIdentity/register",
    {
      email: formData.get("email"),
      password: formData.get("password"),
      permission: formData.get("permission"),
    },
    {
      headers: {
        "Content-Type": "application/json",
      },
    },
  );
  console.log({ response });
  return response.data;
}
// todo 這邊用 ref 來處理
// todo 不要用 form 去接資料 直接用 useEffect 處理
// todo 前面的要不要 也用Ref
// todo here

export default function AccountBlock() {
  const [emailError, setEmailError] = useState(false);
  const [emailErrorMessage, setEmailErrorMessage] = useState("");
  const [passwordError, setPasswordError] = useState(false);
  const [passwordErrorMessage, setPasswordErrorMessage] = useState("");
  const [age, setAge] = useState("guest");
  const fetcher = useFetcher();
  const responseData = fetcher.data;
  console.log({ responseData });

  const validateInputs = () => {
    const email = document.getElementById("email") as HTMLInputElement;
    const password = document.getElementById("password") as HTMLInputElement;

    let isValid = true;

    if (!email.value || !/\S+@\S+\.\S+/.test(email.value)) {
      setEmailError(true);
      setEmailErrorMessage("Please enter a valid email address.");
      isValid = false;
    } else {
      setEmailError(false);
      setEmailErrorMessage("");
    }

    if (!password.value || password.value.length < 6) {
      setPasswordError(true);
      setPasswordErrorMessage("Password must be at least 6 characters long.");
      isValid = false;
    } else {
      setPasswordError(false);
      setPasswordErrorMessage("");
    }

    return isValid;
  };

  const handleSubmit = (event: React.FormEvent<HTMLFormElement>) => {
    if (emailError || passwordError) {
      event.preventDefault();
      return;
    }
    const submitFormData = new FormData(event.currentTarget);
    fetcher.submit(submitFormData, { method: "post", action: "/account" });
  };

  const handleChange = (event: SelectChangeEvent) => {
    setAge(event.target.value as string);
  };
  //todo
  // https://mui.com/material-ui/react-snackbar/
  // todo
  // 這邊用 action
  //
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
        <fetcher.Form method="post" onSubmit={handleSubmit}>
          <Box
            component="form"
            sx={{ display: "flex", flexDirection: "column", gap: 2 }}
          >
            <FormControl>
              <FormLabel htmlFor="email">Email</FormLabel>
              <TextField
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
                value={age}
                onChange={handleChange}
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
              variant="contained"
              onClick={validateInputs}
            >
              註冊
            </Button>
          </Box>
        </fetcher.Form>
      </Card>
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
  height: "calc((1 - var(--template-frame-height, 0)) * 100dvh)",
  minHeight: "100%",
  padding: theme.spacing(2),
  [theme.breakpoints.up("sm")]: {
    padding: theme.spacing(4),
  },
}));
