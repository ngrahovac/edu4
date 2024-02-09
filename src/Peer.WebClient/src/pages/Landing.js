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
import NeutralButton from '../comps/buttons/NeutralButton'

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
                    <LandingNavbarItem>Ways to support</LandingNavbarItem>
                    <TertiaryButton
                        text="Enter"
                        onClick={() => loginWithRedirect({ prompt: 'login' })}>
                    </TertiaryButton>
                </div>
            </LandingNavbar>

            <div className='flex flex-col pt-24 pb-16 gap-y-20 h-full'>
                <div className='flex flex-col gap-y-4 justify-start items-start justify-items-start'>
                    <Slogan></Slogan>

                    <p className='md:text-lg text-left w-full text-gray-600'>
                        Whether you're a student, researcher, industry professional - or all of the above -
                        <span className='font-bold'> peer </span>
                        helps you find your people.&nbsp;
                    </p>
                </div>
                
                <div className='flex flex-row gap-x-2'>
                <AccentButton text="Stay in the Loop"></AccentButton>
                <BorderlessButton text="Learn more"></BorderlessButton>
                </div>
            </div>
        </LandingLayout>
    )
}

export default Landing
