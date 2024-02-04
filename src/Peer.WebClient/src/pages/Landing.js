import React, { useState, useEffect } from 'react';
import { useAuth0 } from '@auth0/auth0-react';
import jwtDecode from 'jwt-decode';
import Slogan from '../comps/landing/Slogan';
import LandingLayout from '../layout/LandingLayout';
import AccentButton from '../comps/buttons/AccentButton';
import LandingNavbar from '../comps/landing/LandingNavbar';
import BorderlessButton from '../comps/buttons/BorderlessButton'
import { SectionTitle } from '../layout/SectionTitle'
import Lang from '../comps/landing/Lang';
import Nudge from '../comps/landing/Nudge';
import PrimaryButton from '../comps/buttons/PrimaryButton';
import LogoIcon from '../comps/nav/LogoIcon'
import TertiaryButton from '../comps/buttons/TertiaryButton'
import CTA from '../comps/landing/CTA';
import LandingFlair from '../comps/landing/LandingFlair';
import PainPoints from './PainPoints';
import LandingNavbarItem from '../comps/landing/LandingNavbarItem';
import LandingNavbarSeparator from '../comps/landing/LandingNavbarSeparator';

const Landing = () => {
    const {
        isAuthenticated,
        isLoading,
        getAccessTokenSilently,
        loginWithRedirect
    } = useAuth0();

    const [selectedHatType, setSelectedHatType] = useState(undefined);

    useEffect(() => {
        if (isLoading || !isAuthenticated)
            return;

        (async () => {
            try {
                let accessToken = await getAccessTokenSilently({
                    audience: process.env.REACT_APP_EDU4_API_IDENTIFIER
                });

                let decodedAccessToken = jwtDecode(accessToken);

                if (!process.env.NODE_ENV || process.env.NODE_ENV === 'development') {
                    window.location.href = "/welcome";
                    return;
                }

                let permissions = decodedAccessToken.permissions;

                if (permissions.includes("contribute")) {
                    window.location.href = "/welcome";

                } else {
                    window.location.href = "/signup";
                }
            } catch (ex) {
                console.error("error fetching access token", ex)
            }
        })();
    });

    const studentUseCases = (
        <div className='flex flex-row w-full space-x-16'>
            <Nudge
                problem="You’ve been thinking about an idea for months, but have nobody to help bring it to life?"
                solution="Describe the collaborators you need and reach people you couldn't have otherwise.">
            </Nudge>

            <Nudge
                problem="Your team is working on a SaaS solution, but not one of you can design to save their life?"
                solution="Find designers and frontend developers that will bring everything to another level.">
            </Nudge>

            <Nudge
                problem="Your team is cooking up something great, but could use the help of a seasoned professional?"
                solution="Find the perfect mentor.">
            </Nudge>
        </div>);

    const academicUseCases = (
        <div className='flex flex-row w-full space-x-16'>
            <Nudge
                problem="You’re coordinating a project and there is room for a student contributor?"
                solution="Let them know about the amazing research opportunity.">
            </Nudge>

            <Nudge
                problem="Your telco project needs a mobile app?"
                solution="Find a student that would love to help out.">
            </Nudge>

            <Nudge
                problem="Enjoy mentoring?"
                solution="Discover ambitious teams and individuals and guide them with your expertise.">
            </Nudge>
        </div>);

    return (
        <LandingLayout>
            <LandingNavbar>
                <div>
                    <LogoIcon></LogoIcon>
                </div>
                <div className='flex gap-x-8 align-middle items-center'>
                    {/*
                    <BorderlessButton
                        text="Learn more">
                    </BorderlessButton>
    */}
                    {/*
                    <AccentButton
                        text="Support"
                        icon={
                            <svg xmlns="http://www.w3.org/2000/svg" fill="currentColor" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" className="w-6 h-6">
                                <path strokeLinecap="round" strokeLinejoin="round" d="M21 8.25c0-2.485-2.099-4.5-4.688-4.5-1.935 0-3.597 1.126-4.312 2.733-.715-1.607-2.377-2.733-4.313-2.733C5.1 3.75 3 5.765 3 8.25c0 7.22 9 12 9 12s9-4.78 9-12z" />
                            </svg>
                        }>
                    </AccentButton>
                    */}


                    <LandingNavbarItem>Learn more</LandingNavbarItem>
                    <LandingNavbarItem>Ways to support</LandingNavbarItem>
                    <TertiaryButton
                        text="Enter"
                        onClick={() => loginWithRedirect({ prompt: 'login' })}>
                    </TertiaryButton>
                </div>
            </LandingNavbar>

            <div className='flex flex-col pt-24 pb-8 gap-y-24 justify-start h-screen'>
                <div className='flex flex-col gap-y-8 justify-start items-start justify-items-start'>
                    <Slogan></Slogan>

                    <p className='text-lg text-center w-full text-gray-600'>
                        Whether you're a student, researcher, industry professional - or all of the above -
                        <span className='font-bold'> peer </span>
                        helps you find your people.&nbsp;
                        <span className='text-indigo-500 hover:text-indigo-600 cursor-pointer font-medium'>Learn how.</span>
                    </p>
                </div>

                <CTA></CTA>
            </div>
        </LandingLayout>
    )
}

export default Landing
