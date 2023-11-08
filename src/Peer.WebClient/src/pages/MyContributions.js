import React, { useEffect } from 'react'
import { useState } from 'react'
import SingleColumnLayout from '../layout/SingleColumnLayout'
import ContributionsMenu from '../comps/contributions/ContributionsMenu'
import MyProjects from '../comps/contributions/MyProjects'
import { useAuth0 } from '@auth0/auth0-react'
import { getAuthored, getById } from '../services/ProjectsService'
import { getCollaborations, getOwnCollaborations } from '../services/CollaboratorsService'
import { successResult, failureResult, errorResult } from '../services/RequestResult'
import MyCollaborations from '../comps/contributions/MyCollaborations'

const MyContributions = () => {
    const sections = [
        "projects",
        "collaborations"
    ]

    const [selectedSection, setSelectedSection] = useState(sections[1]);
    const [authoredProjects, setAuthoredProjects] = useState(undefined);

    const [collaborations, setCollaborations] = useState(undefined);
    const [collaborationProjects, setCollaborationProjects] = useState(undefined);

    const [loading, setLoading] = useState(true);

    const { getAccessTokenSilently } = useAuth0();

    const fetchAuthoredProjects = async () => {
        try {
            setLoading(true);

            let token = await getAccessTokenSilently({
                audience: process.env.REACT_APP_EDU4_API_IDENTIFIER
            });

            let result = await getAuthored(token);

            if (result.outcome === successResult) {
                console.log("authored projects fetched successfully");
                let projects = await result.payload;
                setAuthoredProjects(projects);
            } else if (result.outcome === failureResult) {
                console.log("failure fetching authored projects");
            } else if (result.outcome === errorResult) {
                console.log("error fetching authored projects");
            }
        } catch (ex) {
            console.log("exception while fetching authored projects", ex);
        } finally {
            setLoading(false);
        }
    }

    const fetchCollaborations = async () => {
        try {
            setLoading(true);

            let token = await getAccessTokenSilently({
                audience: process.env.REACT_APP_EDU4_API_IDENTIFIER
            });

            let result = await getOwnCollaborations(token);

            if (result.outcome === successResult) {
                console.log("own collaborations fetched successfully");
                let collaborations = await result.payload;

                let collaborationProjects = [];

                for (let collaboration of collaborations) {
                    result = await getById(collaboration.projectId, token);

                    if (result.successResult) {
                        let project = await result.payload;
                        collaborationProjects.push(project);
                    }
                }

                setCollaborationProjects(collaborationProjects);
                setCollaborations(collaborations);
            } else if (result.outcome === failureResult) {
                console.log("failure fetching own collaborations");
            } else if (result.outcome === errorResult) {
                console.log("error fetching own collaborations");
            }
        } catch (ex) {
            console.log("exception while fetching own collaborations", ex);
        } finally {
            setLoading(false);
        }
    }

    useEffect(() => {
        if (selectedSection === "projects") {
            fetchAuthoredProjects();
        } else if (selectedSection === "collaborations") {
            fetchCollaborations();
        }
    }, [selectedSection])


    return (
        <>
            <div className='fixed w-72 top-2 left-0 bottom-0 border-r border-gray-200'>
                <ContributionsMenu
                    onSelectionChange={(selectedSection) => setSelectedSection(selectedSection)}></ContributionsMenu>
            </div>

            <SingleColumnLayout
                title="My contributions">
                <div className='w-full'>
                    {
                        selectedSection === "projects" &&
                        authoredProjects != undefined &&
                        <MyProjects
                            projects={authoredProjects}>
                        </MyProjects>
                    }

                    {
                        selectedSection === "collaborations" &&
                        collaborations != undefined &&
                        collaborationProjects != undefined &&
                        <MyCollaborations
                            collaborations={collaborations}
                            projects={collaborationProjects}>
                        </MyCollaborations>
                    }

                </div>

            </SingleColumnLayout>
        </>
    )
}

export default MyContributions