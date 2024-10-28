import * as React from 'react';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Checkbox from '@mui/material/Checkbox';
import FormControlLabel from '@mui/material/FormControlLabel';
import FormLabel from '@mui/material/FormLabel';
import FormControl from '@mui/material/FormControl';
import Link from '@mui/material/Link';
import TextField from '@mui/material/TextField';
import Typography from '@mui/material/Typography';
import Stack from '@mui/material/Stack';
import MuiCard from '@mui/material/Card';
import { styled } from '@mui/material/styles';
import ForgotPassword from '../ForgotPassword';
import { useFetcher, useSubmit } from 'react-router-dom';
/*
    ? Content-Type 連結紀錄
    application/json
    ? submit 可以 submit FormData
    ? FormData 是甚麼        
*/



export async function action({ params, request }: { params: any, request: any }) {

    //? action request type, params type
    let formData: FormData = await request.formData();
    console.log("params", params)
    console.log("request", formData);

    const response = await fetch("api/account/login", {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ email: formData.get("email"), password: formData.get("password") })
    });
    console.log("response", response)
    const data = await response.json();
    console.log("data", data);
    return null;
}




const Card = styled(MuiCard)(({ theme }) => ({
    display: 'flex',
    flexDirection: 'column',
    alignSelf: 'center',
    width: '100%',
    padding: theme.spacing(4),
    gap: theme.spacing(2),
    margin: 'auto',
    [theme.breakpoints.up('sm')]: {
        maxWidth: '450px',
    },
    boxShadow:
        'hsla(220, 30%, 5%, 0.05) 0px 5px 15px 0px, hsla(220, 25%, 10%, 0.05) 0px 15px 35px -5px',
    ...theme.applyStyles('dark', {
        boxShadow:
            'hsla(220, 30%, 5%, 0.5) 0px 5px 15px 0px, hsla(220, 25%, 10%, 0.08) 0px 15px 35px -5px',
    }),
}));

const SignInContainer = styled(Stack)(({ theme }) => ({
    minHeight: '100%',
    minWidth: "500px",
    padding: theme.spacing(2),
    [theme.breakpoints.up('sm')]: {
        padding: theme.spacing(4),
    },
    '&::before': {
        content: '""',
        display: 'block',
        position: 'absolute',
        zIndex: -1,
        inset: 0,
        backgroundImage:
            'radial-gradient(ellipse at 50% 50%, hsl(210, 100%, 97%), hsl(0, 0%, 100%))',
        backgroundRepeat: 'no-repeat',
        ...theme.applyStyles('dark', {
            backgroundImage:
                'radial-gradient(at 50% 50%, hsla(210, 100%, 16%, 0.5), hsl(220, 30%, 5%))',
        }),
    },
}));

export function SignInPage(props: { disableCustomTheme?: boolean }) {
    const [emailError, setEmailError] = React.useState(false);
    const [emailErrorMessage, setEmailErrorMessage] = React.useState('');
    const [passwordError, setPasswordError] = React.useState(false);
    const [passwordErrorMessage, setPasswordErrorMessage] = React.useState('');
    const [open, setOpen] = React.useState(false);
    let fetcher = useFetcher();



    const handleClickOpen = () => {
        setOpen(true);
    };

    const handleClose = () => {
        setOpen(false);
    };

    const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
        const submitFormData = new FormData(event.currentTarget);
        console.log(submitFormData);
        event.preventDefault();
        if (emailError || passwordError) {
            console.log("error123");
            return;
        }
        let submit = useSubmit();
        console.log("error");
        submit(submitFormData, { method: 'POST', action: "/" });
        console.log("error2");

        /*
        ? submit 可以 submit FormData
        ? FormData 是甚麼
        const response = await fetch("api/account/login", {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ email: submitFormData.get("email"), password: submitFormData.get("password") })
        });
        const data = await response.json();
        */
        /*
            ? Content-Type 連結紀錄
            application/json
            
        */

        //? proxy 設定 紀錄page
        // console.log(data);
        // 做換頁

    };

    const validateInputs = () => {
        const email = document.getElementById('email') as HTMLInputElement;
        const password = document.getElementById('password') as HTMLInputElement;

        let isValid = true;

        if (!email.value || !/\S+@\S+\.\S+/.test(email.value)) {
            setEmailError(true);
            setEmailErrorMessage('Please enter a valid email address.');
            isValid = false;
        } else {
            setEmailError(false);
            setEmailErrorMessage('');
        }

        if (!password.value || password.value.length < 6) {
            setPasswordError(true);
            setPasswordErrorMessage('Password must be at least 6 characters long.');
            isValid = false;
        } else {
            setPasswordError(false);
            setPasswordErrorMessage('');
        }

        return isValid;
    };

    return (
        <>
            <SignInContainer direction="column" justifyContent="space-between">
                <Card variant="outlined">
                    <Typography
                        component="h1"
                        variant="h4"
                        sx={{ width: '100%', fontSize: 'clamp(2rem, 10vw, 2.15rem)' }}
                    >
                        Sign in
                    </Typography>
                    <fetcher.Form method="post" action="/">
                        <Box
                            // component="div"
                            // onSubmit={handleSubmit}
                            // noValidate
                            sx={{
                                display: 'flex',
                                flexDirection: 'column',
                                width: '100%',
                                gap: 2,
                            }}
                        >
                            <FormControl>
                                <Box sx={{ display: 'flex', justifyContent: 'start' }}>
                                    <FormLabel htmlFor="email">Email</FormLabel>
                                </Box>
                                <TextField
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
                                    color={emailError ? 'error' : 'primary'}
                                    sx={{ ariaLabel: 'email' }}
                                />
                            </FormControl>
                            <FormControl>
                                <Box sx={{ display: 'flex', justifyContent: 'space-between' }}>
                                    <FormLabel htmlFor="password">Password</FormLabel>
                                    <Link
                                        component="button"
                                        type="button"
                                        onClick={handleClickOpen}
                                        variant="body2"
                                        sx={{ alignSelf: 'baseline' }}
                                    >
                                        Forgot your password?
                                    </Link>
                                </Box>
                                <TextField
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
                                    color={passwordError ? 'error' : 'primary'}
                                />
                            </FormControl>
                            <FormControlLabel
                                control={<Checkbox value="remember" color="primary" />}
                                label="Remember me"
                            />
                            <ForgotPassword open={open} handleClose={handleClose} />
                            <Button
                                type="submit"
                                fullWidth
                                variant="contained"
                                onClick={validateInputs}
                            >
                                Sign in
                            </Button>
                        </Box>
                    </fetcher.Form>
                </Card>
            </SignInContainer>
        </>
    );
}
