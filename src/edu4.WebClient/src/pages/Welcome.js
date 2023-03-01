import React from 'react';
import { useEffect } from 'react';
import { useAuth0 } from '@auth0/auth0-react';
import jwtDecode from 'jwt-decode';
import EnterButton from '../account/EnterButton';

const Welcome = () => {
    const {
        isAuthenticated,
        isLoading,
        getAccessTokenSilently
    } = useAuth0();

    useEffect(() => {
        if (isLoading)
            return;

        if (!isAuthenticated) {
            console.log("the user is not authenticated")
            return;
        }

        (async () => {
            try {
                let accessToken = await getAccessTokenSilently({
                    audience: process.env.REACT_APP_EDU4_API_IDENTIFIER
                });

                let decodedAccessToken = jwtDecode(accessToken);
                console.log(decodedAccessToken);

                if (!process.env.NODE_ENV || process.env.NODE_ENV === 'development') {
                    console.log("development mode; the logged in user is considered a contributor.");
                    console.log("redirecting to homepage...");
                    window.location.href = "/homepage";
                    return;
                }

                let permissions = decodedAccessToken.permissions;

                if (permissions.includes("contribute")) {
                    console.log("user completed the signup and IS a contributor");
                    console.log("redirecting to homepage...");
                    window.location.href = "/homepage";

                } else {
                    console.log("user did not complete the signup");
                    console.log("redirecting to signup...");
                    window.location.href = "/signup";
                }
            } catch (ex) {
                console.log("error fetching access token")
            }
        })();
    });

    return (
        <>
            <div className='flex flex-row absolute top-2 right-4 z-10'>
                <EnterButton></EnterButton>
            </div>

            <div
                className='w-full h-full text-center pt-64 text-slate-700 absolute bottom-0 align-middle text-3xl'>
                <p className='font-bold text-5xl'>
                    edu4
                </p>
                <p className='mt-8 space-y-2 leading-normal'>
                    Find amazing people
                    <br />
                    and bring your ideas to life.
                </p>
            </div>
        </>
    )
}

export default Welcome
