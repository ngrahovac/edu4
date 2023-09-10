import React, { useState, useEffect } from 'react'
import { successResult } from '../../services/RequestResult';
import SubsectionTitle from '../../layout/SubsectionTitle';
import PositionCard from './PositionCard';
import ProjectDescriptor from './ProjectDescriptor';
import { getContributor } from '../../services/UsersService';
import { useAuth0 } from '@auth0/auth0-react';
import RecommendedFlair from './RecommendedFlair';
import { Link } from 'react-router-dom';
import BorderlessButton from '../buttons/BorderlessButton';

const ProjectCard = ({ project }) => {
    const [author, setAuthor] = useState(undefined);

    const { getAccessTokenWithPopup, getAccessTokenSilently } = useAuth0();

    function fetchAuthor() {
        (async () => {
            let token = await getAccessTokenSilently({
                audience: process.env.REACT_APP_EDU4_API_IDENTIFIER
            });

            var result = await getContributor(token, project.authorUrl);

            if (result.outcome == successResult) {
                setAuthor(result.payload);
            }
        })();
    }

    useEffect(() => {
        fetchAuthor();
    }, [])


    return (
        author != undefined &&

        <div className='relative'>
            <div
                className={`flex flex-col space-y-8 w-full rounded-2xl border ${project.recommended ? "border-lime-500" : "border-gray-300"} px-20 py-16 pb-32 relative hover:bg-gray-50/50`}>

                {/* project descriptors */}
                <div className='flex flex-row flex-wrap space-x-6'>
                    <Link to={project.authorUrl}>
                        <ProjectDescriptor
                            link={true}
                            value={author.fullName}
                            icon={
                                <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className="w-6 h-6">
                                    <path strokeLinecap="round" strokeLinejoin="round" d="M17.982 18.725A7.488 7.488 0 0012 15.75a7.488 7.488 0 00-5.982 2.975m11.963 0a9 9 0 10-11.963 0m11.963 0A8.966 8.966 0 0112 21a8.966 8.966 0 01-5.982-2.275M15 9.75a3 3 0 11-6 0 3 3 0 016 0z" />
                                </svg>
                            }>
                        </ProjectDescriptor>
                    </Link>

                    <ProjectDescriptor
                        value={project.datePosted}
                        icon={
                            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className="w-6 h-6">
                                <path strokeLinecap="round" strokeLinejoin="round" d="M6.75 3v2.25M17.25 3v2.25M3 18.75V7.5a2.25 2.25 0 012.25-2.25h13.5A2.25 2.25 0 0121 7.5v11.25m-18 0A2.25 2.25 0 005.25 21h13.5A2.25 2.25 0 0021 18.75m-18 0v-7.5A2.25 2.25 0 015.25 9h13.5A2.25 2.25 0 0121 11.25v7.5m-9-6h.008v.008H12v-.008zM12 15h.008v.008H12V15zm0 2.25h.008v.008H12v-.008zM9.75 15h.008v.008H9.75V15zm0 2.25h.008v.008H9.75v-.008zM7.5 15h.008v.008H7.5V15zm0 2.25h.008v.008H7.5v-.008zm6.75-4.5h.008v.008h-.008v-.008zm0 2.25h.008v.008h-.008V15zm0 2.25h.008v.008h-.008v-.008zm2.25-4.5h.008v.008H16.5v-.008zm0 2.25h.008v.008H16.5V15z" />
                            </svg>
                        }>
                    </ProjectDescriptor>
                </div>

                <div className='flex flex-col space-y-1'>
                    <p className='text-3xl font-semibold'>{project.title}</p>
                    <p>{project.description}</p>
                </div>

                <div>
                    <SubsectionTitle title={`${project.recommended ? "Recommended positions" : "Positions"}`}></SubsectionTitle>
                </div>


                <div className='flex flex-col space-y-2'>
                    {
                        project.recommended &&

                        project.positions.filter(p => p.recommended).map((p, index) => <div key={index}>
                            <PositionCard position={p}></PositionCard>
                        </div>)
                    }

                    {
                        !project.recommended &&

                        project.positions.map((p, index) => <div key={index}>
                            <PositionCard position={p}></PositionCard>
                        </div>)
                    }
                </div>

                <div className='absolute bottom-8 right-20'>
                    <Link to={project.projectUrl}>
                        <BorderlessButton
                            text="Learn more ➜"
                            onClick={() => { }}>
                        </BorderlessButton>
                    </Link>
                </div>
            </div>

            {
                project.recommended &&
                <div className='absolute top-0 right-0'>
                    <RecommendedFlair></RecommendedFlair>
                </div>
            }
        </div>
    )
}

export default ProjectCard