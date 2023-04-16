import React, { useState } from 'react'
import SingleColumnLayout from '../layout/SingleColumnLayout'
import ProjectDescriptor from '../comps/discover/ProjectDescriptor';
import RecommendedFlair from '../comps/discover/RecommendedFlair';
import SubsectionTitle from '../layout/SubsectionTitle';
import { SectionTitle } from '../layout/SectionTitle';
import PositionCard from '../comps/discover/PositionCard';
import RecommendedPositionCard from '../comps/discover/RecommendedPositionCard';
import Collaborators from '../comps/project/Collaborators';
import Author from '../comps/project/Author';
import Collaborator from '../comps/project/Collaborator';
import BorderlessButtonWithIcon from '../comps/buttons/BorderlessButtonWithIcon';

const Project = () => {

    const avatar = "https://www.gravatar.com/avatar/93e9084aa289b7f1f5e4ab6716a56c3b?s=80";

    const [project, setProject] = useState({
        "id": "74c6895a-1fdd-4149-aeda-f3c71d3a07db",
        "datePosted": "2023-03-31T08:15:37.684Z",
        "title": "Mobile App Development",
        "description": "We are looking for a team of developers to create a mobile app for our company.",
        "authorId": "ce075dea-7706-409e-91e8-7f27580d2da0",
        "authored": true,
        "recommended": false,
        "positions": [
            {
                "id": "aa454375-2469-46c5-83ee-aaee3ad2ee0e",
                "datePosted": "2023-04-14T22:53:55.0701872Z",
                "name": "Android Developer",
                "description": "Responsible for developing and maintaining the Android version of the app.",
                "requirements": {
                    "type": "Student",
                    "parameters": {
                        "type": 0,
                        "studyField": "Computer Science",
                        "academicDegree": 1
                    }
                },
                "recommended": true
            },
            {
                "id": "c76ec284-ed62-4099-b1ed-fc0bef743def",
                "datePosted": "2023-04-14T22:53:55.0702114Z",
                "name": "iOS Developer",
                "description": "Responsible for developing and maintaining the iOS version of the app.",
                "requirements": {
                    "type": "Student",
                    "parameters": {
                        "type": 0,
                        "studyField": "Computer Science",
                        "academicDegree": 2
                    }
                },
                "recommended": true
            },
            {
                "id": "c76ec284-ed62-4099-b1ed-fc0bef743def",
                "datePosted": "2023-04-14T22:53:55.0702114Z",
                "name": "iOS Developer",
                "description": "Not much",
                "requirements": {
                    "type": "Student",
                    "parameters": {
                        "type": 0,
                        "studyField": "Electrical engineering",
                        "academicDegree": 2
                    }
                },
                "recommended": false
            }
        ]
    });

    const [author, setAuthor] = useState({
        name: "John Doe",
    });

    const [collaborators, setCollaborators] = useState([
        {
            name: "John Doe",
            position: "Not much"
        }
    ]);

    return (
        <SingleColumnLayout
            title={project.title}
            description="">
            <div className='flex flex-col space-y-16'>
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

                <div className='space-y-4'>
                    <SectionTitle title="Description"></SectionTitle>
                    <p>{project.description}</p>
                </div>

                <div className='space-y-4'>
                    <SectionTitle title="Collaborators"></SectionTitle>
                    <Collaborators>
                        <Author
                            avatar={avatar}
                            name={author.name}
                            onVisited={() => { }}>
                        </Author>

                        {
                            collaborators.map(c => <>
                                <Collaborator
                                    avatar={avatar}
                                    name={c.name}
                                    position={c.position}
                                    onVisited={() => { }}>
                                </Collaborator>
                            </>)
                        }
                    </Collaborators>
                </div>

                <div>
                    <SectionTitle title="Open positions"></SectionTitle>
                    <div className='flex flex-col space-y-2 mt-4'>
                        {
                            project.positions.map(p => <>
                                {
                                    !p.recommended &&
                                    <PositionCard position={p}></PositionCard>
                                }
                                {
                                    p.recommended &&
                                    <RecommendedPositionCard position={p}></RecommendedPositionCard>
                                }
                            </>)
                        }
                    </div>
                </div>

                {
                    project.authored &&
                    <div className='absolute top-0 right-0 flex flex-row space-x-8'>
                        <BorderlessButtonWithIcon
                            text="Edit"
                            icon={
                                <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className="w-6 h-6">
                                    <path strokeLinecap="round" strokeLinejoin="round" d="M16.862 4.487l1.687-1.688a1.875 1.875 0 112.652 2.652L6.832 19.82a4.5 4.5 0 01-1.897 1.13l-2.685.8.8-2.685a4.5 4.5 0 011.13-1.897L16.863 4.487zm0 0L19.5 7.125" />
                                </svg>
                            }
                            onClick={() => { }}>
                        </BorderlessButtonWithIcon>

                        <BorderlessButtonWithIcon
                            text="Delete"
                            icon={
                                <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className="w-6 h-6">
                                    <path strokeLinecap="round" strokeLinejoin="round" d="M14.74 9l-.346 9m-4.788 0L9.26 9m9.968-3.21c.342.052.682.107 1.022.166m-1.022-.165L18.16 19.673a2.25 2.25 0 01-2.244 2.077H8.084a2.25 2.25 0 01-2.244-2.077L4.772 5.79m14.456 0a48.108 48.108 0 00-3.478-.397m-12 .562c.34-.059.68-.114 1.022-.165m0 0a48.11 48.11 0 013.478-.397m7.5 0v-.916c0-1.18-.91-2.164-2.09-2.201a51.964 51.964 0 00-3.32 0c-1.18.037-2.09 1.022-2.09 2.201v.916m7.5 0a48.667 48.667 0 00-7.5 0" />
                                </svg>
                            }
                            onClick={() => { }}>
                        </BorderlessButtonWithIcon>
                    </div>
                }
            </div>
        </SingleColumnLayout>
    )
}

export default Project