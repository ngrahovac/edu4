import React from 'react'
import SingleColumnLayout from '../layout/SingleColumnLayout'
import { SectionTitle } from '../layout/SectionTitle'
import ProjectCard from '../comps/discover/ProjectCard';

const Discover = () => {
    const projects = [
        {
            "id": "string",
            "datePosted": "01/02/03",
            "title": "platform development and maintenance",
            "description": "We are looking for new members to join the platform development team",
            "authorId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
            "positions": [
                {
                    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
                    "datePosted": "2023-03-17T16:07:08.110Z",
                    "name": ".NET Backend Developer",
                    "description": "Develop and maintain platform backend",
                    "requirements": {
                        "type": "Student",
                        "parameters": {
                            "studyField": "Software Engineering",
                            "academicDegree": "Bachelor's"
                        }
                    }
                }
            ]
        }
    ];

    return (
        <SingleColumnLayout
            title="Discover projects"
            description="Something encouraging here">

            <div className='mt-16'>
                {
                    projects.map(p => <>
                        <div>
                            <ProjectCard project={p}></ProjectCard>
                        </div>
                    </>)
                }
            </div>

        </SingleColumnLayout>
    )
}

export default Discover