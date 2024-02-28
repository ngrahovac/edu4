import React from 'react'
import { useState, useEffect } from 'react';
import { useSearchParams } from 'react-router-dom';
import SpinnerLayout from '../layout/SpinnerLayout';
import { BeatLoader } from 'react-spinners';
import { useAuth0 } from '@auth0/auth0-react';
import ProjectCard from '../comps/discover/ProjectCard';
import SingleColumnLayout from '../layout/SingleColumnLayout';
import { me } from '../services/UsersService';
import { successResult, failureResult, errorResult } from '../services/RequestResult';
import { discover } from '../services/ProjectsService';
import RefineButton from '../comps/discover/RefineButton'
import ProjectSearchParam from '../comps/search/ProjectSearchParam';
import DiscoveryParametersSidebar from '../comps/discover/DiscoveryRefinementSidebar';

const Search = () => {

    const [search, setSearch] = useSearchParams();
    const [keyword, setKeyword] = useState(undefined);
    const [sort, setSort] = useState(undefined);
    const [hat, setHat] = useState(undefined);

    const [ownHats, setOwnHats] = useState(undefined);
    const [projects, setProjects] = useState(undefined);

    const [searchRefinementsFormVisible, setSearchRefinementsFormVisible] = useState(false);

    const [pageLoading, setPageLoading] = useState(true);
    const [projectsLoading, setProjectsLoading] = useState(true);

    const { getAccessTokenSilently } = useAuth0();

    useEffect(() => {
        console.log(search)
        console.log(search.get('hat'))
        setKeyword(search.get('keyword'));
        setSort(search.get('sort'));
        setHat(search.get('hat') ? ownHats.find(h => h.type == search.get('hat')) : undefined);
    }, [search])

    function setSearchParam(key, value) {
        const newParams = new URLSearchParams();

        search.forEach((value, k) => {
            if (k !== key) {
                newParams.append(k, value);
            }
        });

        if (value !== undefined)
            newParams.set(key, value);

        setSearch(newParams);
    }

    function toggleSearchRefinementsFormVisibility() {
        setSearchRefinementsFormVisible(!searchRefinementsFormVisible);
    }

    function handleDiscoveryRefinementsChange(keyword, sort, hat) {
        const newParams = new URLSearchParams();
        keyword && newParams.append('keyword', keyword);
        sort && newParams.append('sort', sort);
        hat && newParams.append('hat', hat.type);

        setSearch(newParams);
    }

    useEffect(() => {
        const fetchOwnHats = () => {
            (async () => {
                setPageLoading(true);

                try {
                    let token = await getAccessTokenSilently({
                        audience: process.env.REACT_APP_EDU4_API_IDENTIFIER
                    });

                    let result = await me(token);
                    setPageLoading(false);

                    if (result.outcome === successResult) {
                        setOwnHats(result.payload.hats);
                    } else {
                        console.log("error fetching users hats");
                    }
                } catch (ex) {
                    console.log(ex);
                } finally {
                    setPageLoading(false);
                }
            })();
        }

        fetchOwnHats();
    }, [getAccessTokenSilently])

    useEffect(() => {
        const handleDiscoveryRefinementsChange = () => {
            (async () => {
                try {
                    setProjectsLoading(true);

                    let token = await getAccessTokenSilently({
                        audience: process.env.REACT_APP_EDU4_API_IDENTIFIER
                    });

                    let result = await discover(keyword, sort, hat, token);
                    setProjectsLoading(false);

                    if (result.outcome === successResult) {
                        var projects = result.payload;
                        setProjects(projects);
                    } else if (result.outcome === failureResult) {
                        console.log("failure");
                    } else if (result.outcome === errorResult) {
                        console.log("error");
                    }
                } catch (ex) {
                    console.log("exception", ex);
                } finally {
                    setProjectsLoading(false);
                }
            })();
        }

        handleDiscoveryRefinementsChange();
    }, [keyword, sort, hat])

    if (pageLoading) {
        return (
            <SpinnerLayout>
                <BeatLoader
                    loading={pageLoading}
                    size={24}
                    color="blue">
                </BeatLoader>
            </SpinnerLayout>
        )
    }

    const searchResults = projectsLoading ?
        <BeatLoader></BeatLoader> :
        projects && projects.length ?
            <div className='flex flex-col space-y-8'>
                {
                    projects.map((p, index) => <div key={index}>
                        <ProjectCard project={p}></ProjectCard>
                    </div>)
                }
            </div> :
            <p>There are currently no projects satisfying the criteria.</p>;

    return (
        ownHats &&

        <SingleColumnLayout
            title={`Search results ${keyword ? `for ${keyword}` : ''}`}>
            <div className='flex flex-col gap-y-8'>
                <div className='flex gap-x-2'>
                    {
                        keyword &&
                        <ProjectSearchParam
                            onRemove={() => { setSearchParam('keyword', undefined) }}>
                            contains "{keyword}"
                        </ProjectSearchParam>
                    }

                    {
                        sort &&
                        <ProjectSearchParam
                            onRemove={() => { setSearchParam('sort', undefined) }}>
                            {`${sort === "asc" ? "oldest posted first" : "newest posted first"}`}
                        </ProjectSearchParam>
                    }

                    {
                        hat &&
                        <ProjectSearchParam
                            onRemove={() => { setSearchParam('hat', undefined) }}>
                            for a {hat.type} like me
                        </ProjectSearchParam>
                    }
                </div>

                {searchResults}

                {
                    searchRefinementsFormVisible &&
                    <div className='fixed left-0 top-0'>
                        <DiscoveryParametersSidebar
                            keyword={keyword}
                            sort={sort}
                            hat={hat}
                            hats={ownHats}
                            onModalClosed={() => setSearchRefinementsFormVisible(false)}
                            onDiscoveryParametersChanged={handleDiscoveryRefinementsChange}>
                        </DiscoveryParametersSidebar>
                    </div>
                }
            </div>
        </SingleColumnLayout >
    )
}

export default Search