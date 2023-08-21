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

const Landing = () => {
    const {
        isAuthenticated,
        isLoading,
        getAccessTokenSilently
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
                    window.location.href = "/homepage";
                    return;
                }

                let permissions = decodedAccessToken.permissions;

                if (permissions.includes("contribute")) {
                    window.location.href = "/homepage";

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
                <BorderlessButton
                    text="Learn more">
                </BorderlessButton>

                <AccentButton
                    text="Support"
                    icon={
                        <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" className="w-6 h-6">
                            <path strokeLinecap="round" strokeLinejoin="round" d="M21 8.25c0-2.485-2.099-4.5-4.688-4.5-1.935 0-3.597 1.126-4.312 2.733-.715-1.607-2.377-2.733-4.313-2.733C5.1 3.75 3 5.765 3 8.25c0 7.22 9 12 9 12s9-4.78 9-12z" />
                        </svg>
                    }>
                </AccentButton>

                <Lang></Lang>
            </LandingNavbar>

            <div className='flex flex-col items-center space-y-16'>
                {/* giant logo + slogan */}
                <div className='w-full'>
                    <div className='flex flex-col space-y-8'>
                        <img
                            width={420}
                            height={120}
                            src='https://placehold.co/420x120?text=big+logo+here'>
                        </img>

                        <Slogan></Slogan>
                    </div>
                </div>

                {/* one-line description */}
                <p className='text-xl text-center'>
                    Whether you're a student, researcher, industry professional - or all of the above -
                    <span className='font-bold'> peer </span>
                    helps you find your people.
                </p>

                {/* pain points */}
                <div className='flex flex-col items-center'>
                    <div className='flex flex-row items-center text-lg mb-8 w-96'>
                        <p className='w-32'>I am a...</p>
                        <select
                            onChange={e => setSelectedHatType(e.target.value)}
                            value={selectedHatType}
                            className="w-full rounded-full px-4 border-gray-300 shadow-sm focus:border-indigo-600 focus:ring focus:ring-indigo-200 focus:ring-opacity-10">
                            <option value={undefined}>Choose..</option>
                            <option value="Student">Student</option>
                            <option value="Researcher / Academic">Researcher / Academic</option>
                        </select>
                    </div>

                    <div className='text-lg'>
                        {
                            selectedHatType == "Student" ?
                                studentUseCases :
                                selectedHatType == "Researcher / Academic" ?
                                    academicUseCases :
                                    null
                        }
                    </div>
                </div>

                {/* CTA */}
                <div className='flex flex-col items-center space-y-4 absolute bottom-16'>
                    <SectionTitle title="Join private beta"></SectionTitle>
                    <label>
                        <form
                            onSubmit={e => e.preventDefault()}
                            className='flex flex-row rounded-full border border-gray-400 justify-between px-4 py-2 w-full'>
                            <input
                                type="email"
                                required={true}
                                name="title"
                                placeholder='Your email here..'
                                className="border-none focus:outline-none focus:ring-none text-lg">
                            </input>
                            <button 
                                className='text-lg font-bold px-4 rounded-full'>
                                Go
                            </button>
                        </form>
                    </label>
                </div>
            </div>
        </LandingLayout>
    )
}

export default Landing
