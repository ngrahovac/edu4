import React, { useState, useEffect } from 'react'
import { successResult } from '../../services/RequestResult';
import SubsectionTitle from '../../layout/SubsectionTitle';
import PositionCard from './PositionCard';
import ProjectDescriptor from './ProjectDescriptor';
import { getContributor } from '../../services/UsersService';
import { useAuth0 } from '@auth0/auth0-react';
import RecommendedFlair from './RecommendedFlair';
import { Link } from 'react-router-dom';
import ProjectTitle from './ProjectTitle';

const ProjectCard = (props) => {

    const {
        project
    } = props;

    const [author, setAuthor] = useState(undefined);

    const { getAccessTokenSilently } = useAuth0();

    useEffect(() => {
        function fetchAuthor() {
            (async () => {
                let token = await getAccessTokenSilently({
                    audience: process.env.REACT_APP_EDU4_API_IDENTIFIER
                });

                var result = await getContributor(token, project.authorUrl);

                if (result.outcome === successResult) {
                    setAuthor(result.payload);
                }
            })();
        }

        fetchAuthor();
    }, [getAccessTokenSilently, project])

    return (
        author !== undefined &&

        <div className='border-4 border-pink-500'>
            <div className="flex flex-col gap-y-8 px-20 py-8">
                <div>
                    <div className='flex justify-between items-center flex-wrap gap-y-2'>
                        <div className='flex gap-x-4'>
                            <ProjectDescriptor
                                link={true}
                                value={author.fullName}
                                icon={
                                    <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="gray" className="w-12 h-12">
                                        <path strokeLinecap="round" strokeLinejoin="round" d="M17.982 18.725A7.488 7.488 0 0012 15.75a7.488 7.488 0 00-5.982 2.975m11.963 0a9 9 0 10-11.963 0m11.963 0A8.966 8.966 0 0112 21a8.966 8.966 0 01-5.982-2.275M15 9.75a3 3 0 11-6 0 3 3 0 016 0z" />
                                    </svg>
                                }>
                            </ProjectDescriptor>

                            <ProjectDescriptor
                                value={<p><span className='font-black'>7</span> days ago</p>}>
                            </ProjectDescriptor>
                        </div>

                        {
                            project.recommended &&
                            <div className='flex flex-end'>
                                <RecommendedFlair></RecommendedFlair>
                            </div>
                        }

                    </div>
                </div>

                <div className='flex flex-col gap-y-2'>
                    <ProjectTitle>{project.title}</ProjectTitle>

                    <div className='flex gap-x-4 justify-between flex-wrap gap-y-2'>
                        <ProjectDescriptor
                            icon={
                                <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={3} stroke="lightgray" className="w-5 h-5">
                                    <path strokeLinecap="round" strokeLinejoin="round" d="M12 6v6h4.5m4.5 0a9 9 0 1 1-18 0 9 9 0 0 1 18 0Z" />
                                </svg>
                            }
                            value={<p>starts <span className='font-black'>oct '24</span></p>}>
                        </ProjectDescriptor>

                        <ProjectDescriptor
                            icon={
                                <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={3} stroke="lightgray" className="w-5 h-5">
                                    <path strokeLinecap="round" strokeLinejoin="round" d="M12 6v6h4.5m4.5 0a9 9 0 1 1-18 0 9 9 0 0 1 18 0Z" />
                                </svg>
                            }
                            value={<p><span className='font-black'>2</span> open positions</p>}>
                        </ProjectDescriptor>

                        <ProjectDescriptor
                            icon={
                                <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={3} stroke="lightgray" className="w-5 h-5">
                                    <path strokeLinecap="round" strokeLinejoin="round" d="M12 6v6h4.5m4.5 0a9 9 0 1 1-18 0 9 9 0 0 1 18 0Z" />
                                </svg>
                            }
                            value={<p><span className='font-black'>7</span> collaborators</p>}>
                        </ProjectDescriptor>
                    </div>
                </div>

                <div className='flex flex-col gap-y-4'>
                    <SubsectionTitle title={`${project.recommended ? "Recommended positions" : "Positions"}`}></SubsectionTitle>

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
                </div>

                <div className='flex items-start'>
                    <ProjectDescriptor
                        value={<p>... and <span className='font-black'>3</span> more</p>}>
                    </ProjectDescriptor>
                </div>

                <div className='flex flex-row-reverse'>
                    <Link to={project.projectUrl}>
                        <p className='uppercase tracking-wide font-semibold text-indigo-500'>Learn more</p>
                    </Link>
                </div>
            </div>
        </div>
    )
}

export default ProjectCard