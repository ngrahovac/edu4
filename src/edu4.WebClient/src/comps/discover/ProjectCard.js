import React from 'react'
import { SectionTitle } from '../../layout/SectionTitle';
import SubsectionTitle from '../../layout/SubsectionTitle';
import NeutralButton from '../buttons/NeutralButton';
import PositionCard from './PositionCard';
import ProjectDescriptor from './ProjectDescriptor';

const ProjectCard = (props) => {
    const { project } = props;

    return (
        <div
            className='w-full rounded-2xl shadow-xl shadow-indigo-300/10 border border-gray-200 px-20 py-16 pb-44 relative'>
            <div className='flex flex-row flex-wrap space-x-6 mb-2'>
                <ProjectDescriptor
                    value={project.authorId.substring(0, 10)}
                    icon=
                    {
                        <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className="w-6 h-6">
                            <path strokeLinecap="round" strokeLinejoin="round" d="M17.982 18.725A7.488 7.488 0 0012 15.75a7.488 7.488 0 00-5.982 2.975m11.963 0a9 9 0 10-11.963 0m11.963 0A8.966 8.966 0 0112 21a8.966 8.966 0 01-5.982-2.275M15 9.75a3 3 0 11-6 0 3 3 0 016 0z" />
                        </svg>
                    }>
                </ProjectDescriptor>

                <ProjectDescriptor
                    value={project.datePosted}
                    icon=
                    {<svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className="w-6 h-6">
                        <path strokeLinecap="round" strokeLinejoin="round" d="M6.75 3v2.25M17.25 3v2.25M3 18.75V7.5a2.25 2.25 0 012.25-2.25h13.5A2.25 2.25 0 0121 7.5v11.25m-18 0A2.25 2.25 0 005.25 21h13.5A2.25 2.25 0 0021 18.75m-18 0v-7.5A2.25 2.25 0 015.25 9h13.5A2.25 2.25 0 0121 11.25v7.5m-9-6h.008v.008H12v-.008zM12 15h.008v.008H12V15zm0 2.25h.008v.008H12v-.008zM9.75 15h.008v.008H9.75V15zm0 2.25h.008v.008H9.75v-.008zM7.5 15h.008v.008H7.5V15zm0 2.25h.008v.008H7.5v-.008zm6.75-4.5h.008v.008h-.008v-.008zm0 2.25h.008v.008h-.008V15zm0 2.25h.008v.008h-.008v-.008zm2.25-4.5h.008v.008H16.5v-.008zm0 2.25h.008v.008H16.5V15z" />
                    </svg>}>
                </ProjectDescriptor>
            </div>

            <p className='text-3xl mb-2 font-semibold'>{project.title}</p>
            <p className='mb-10'>{project.description}</p>

            <div className='mb-8'>
                <SubsectionTitle title="Recommended positions"></SubsectionTitle>
            </div>


            {
                project.positions.map(p => <>
                    <PositionCard position={p}></PositionCard>
                </>)
            }

            <div
                className='absolute bottom-8 right-20'>
                <NeutralButton text="Learn more"></NeutralButton>
            </div>

        </div>
    )
}

export default ProjectCard